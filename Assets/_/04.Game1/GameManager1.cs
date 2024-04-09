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
    // ������ ������ ���� ���ڿ� ������ ���� Ÿ�̸�
    // Ÿ�̸� ����
    public float interval = 10.0f;
    public Dictionary<string, int> playerScorePair = new Dictionary<string, int>();
    [SerializeField] GameObject gameEndTxt;
    // ���� ����Ʈ
    [SerializeField]private List<Player> players;
    // ������
    private Player questionMaster;
    private float timer;
    private PlayerNamer namer;
    void Start()
    {
        players = new List<Player>();
        pen = FindObjectOfType<UVDrawing>();
        namer = GetComponent<PlayerNamer>();
        // ���� ����Ʈ�� ���� ���̺� �ʱ�ȭ
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
        // ������ Ŭ���̾�Ʈ������ ������ ������ ���� ���� ���� ����

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

            // �ش� ������ ����Ʈ���� ����
            players.RemoveAt(index);
            Debug.LogWarning(questionMaster.NickName);
            photonView.RPC("UpdateQuestionMaster", RpcTarget.AllBuffered, questionMaster.NickName);
        }
        else
        {
            // ��� ������ �����ڷ� ������ ���, ���� ����Ʈ�� �ٽ� �ʱ�ȭ
            foreach (var player in PhotonNetwork.PlayerList)
            {
                players.Add(player);
            }
        }
    }
    void SaveMyPoint()
    {
        int myPoint = playerScorePair[PhotonNetwork.NickName]; // ����� ������ �����ɴϴ�.

        PlayFabClientAPI.UpdateUserData(new UpdateUserDataRequest()
        {
            Data = new Dictionary<string, string>()
        {
            {"RankPoint", myPoint.ToString()} // ������ ���ڿ��� ��ȯ�Ͽ� �����մϴ�.
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

            // �ش� ������ ����Ʈ���� ����
            answerTable.RemoveAt(index);

            // ������ �����ڿ��Ը� ����
            photonView.RPC("UpdateAnswerTxt", RpcTarget.All, answer, questionMaster);
        }
        //��� ���� ����. ��������
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
        Debug.Log("������");
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
        // ���� �׸� ������ ���� �׸��ϴ�.
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
