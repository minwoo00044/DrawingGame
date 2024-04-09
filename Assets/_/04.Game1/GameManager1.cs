using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using PlayFab.ClientModels;
using PlayFab;
using System;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    public static string answer;
    public TMP_Text timerTxt;
    public UVDrawing pen;
    public List<string> answerTable = new List<string>();
    public TMP_Text answerTxt;
    // 출제자 지정과 정답 문자열 제공을 위한 타이머
    // 타이머 간격
    public float interval = 10.0f;
    public Dictionary<string, int> playerScorePair = new Dictionary<string, int>();
    [SerializeField] GameObject gameEndTxt;
    // 유저 리스트
    [SerializeField]private List<Player> players;
    // 출제자
    private Player questionMaster;
    private float timer;
    private PlayerNamer namer;
    void Start()
    {
        players = new List<Player>();
        pen = FindObjectOfType<UVDrawing>();
        namer = GetComponent<PlayerNamer>();
        // 유저 리스트와 정답 테이블 초기화
        foreach (var player in PhotonNetwork.PlayerList)
        {
            players.Add(player);
            playerScorePair.Add(player.NickName, 0);
        }
        timer = interval;
        Invoke("Choose", 1f);

    }
    public void Choose()
    {
        if (PhotonNetwork.IsMasterClient)
        {

            AssignQuestionMaster();
            ProvideAnswer();
        }
    }
    [PunRPC]
    public void Correct(string name, int idx)
    {
        if (name == questionMaster.NickName)
            return;
        timer = 0.1f;
        playerScorePair[name] += 1;
        namer.SetUserScore(name, idx);
    }
    void Update()
    {
        // 마스터 클라이언트에서만 출제자 지정과 정답 제공 로직 실행

        timer -= Time.deltaTime;
        SetTimer(timer);
        if (timer <= 0)
        {
            timer = interval;
            if (PhotonNetwork.IsMasterClient)
            {
                Choose();
                photonView.RPC("TexReset", RpcTarget.All);
            }
        }
    }
    void SetTimer(float time)
    {
        timerTxt.text = time.ToString("F");
    }
    void AssignQuestionMaster()
    {
        if (players.Count > 0)
        {

            int index = Random.Range(0, players.Count);
            questionMaster = players[index];

            // 해당 유저를 리스트에서 제거
            players.RemoveAt(index);
            Debug.LogWarning(questionMaster.NickName);
            photonView.RPC("UpdateQuestionMaster", RpcTarget.AllBuffered, questionMaster.NickName);
        }
        else
        {
            // 모든 유저가 출제자로 지정된 경우, 유저 리스트를 다시 초기화
            foreach (var player in PhotonNetwork.PlayerList)
            {
                players.Add(player);
            }
        }
    }
    void SaveMyPoint()
    {
        int myPoint = playerScorePair[PhotonNetwork.NickName]; // 당신의 점수를 가져옵니다.

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {"RankPoint", myPoint.ToString()} // 점수를 문자열로 변환하여 저장합니다.
        }
        },
        result => Debug.Log("Successfully updated user data"),
        error => Debug.LogError(error.GenerateErrorReport()));
    }



    [PunRPC]
    void UpdateQuestionMaster(string playerName)
    {
        timer = interval;
        SetTimer(timer);
        pen.canDrawing = false;
        answer = string.Empty;
        answerTxt.text = string.Empty;
        Debug.Log(playerName + " is the question master.");
    }
    void ProvideAnswer()
    {
        if (answerTable.Count > 0)
        {
            int index = Random.Range(0, answerTable.Count);
            string answer = answerTable[index];

            // 해당 정답을 리스트에서 제거
            answerTable.RemoveAt(index);

            // 정답은 출제자에게만 제공
            photonView.RPC("UpdateAnswerTxt", RpcTarget.All, answer, questionMaster);
        }
        //모든 정답 소진. 게임종료
        else
        {
            photonView.RPC("GameEnd", RpcTarget.All);
        }
    }
    [PunRPC]
    void GameEnd()
    {
        gameEndTxt.GetComponent<Button>().onClick.AddListener(() => PhotonNetwork.LeaveRoom());

        gameEndTxt.SetActive(true);
        SaveMyPoint();
        Time.timeScale = 0;
    }
    public override void OnLeftRoom()
    {
        Debug.Log("나갔다");
        Utils.SceneChange(SceneNum.Lobby);
    }
    [PunRPC]
    void UpdateAnswerTxt(string _answer, Player target)
    {
        answer = _answer;
        questionMaster = target;
        if(PhotonNetwork.LocalPlayer == target)
        {
            answerTxt.text = _answer;
            pen.canDrawing = true;
        }
    }

    [PunRPC]
    void SyncDrawing(Vector2 fromUv, Vector2 toUv, float[] colorArray, int brushSize)
    {
        Color color = new Color(colorArray[0], colorArray[1], colorArray[2], colorArray[3]);
        // 받은 그림 정보로 선을 그립니다.
        pen.DrawLine(fromUv, toUv, color, brushSize);
    }

    [PunRPC]
    void SetTex()
    {
        pen.drawingTexture.Apply();
    }
    [PunRPC]
    void TexReset()
    {
        pen.ResetDrawingTexture();
    }
}
