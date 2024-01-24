using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    // 유저 리스트와 정답 테이블
    private List<Player> players;
    public List<string> answerTable = new List<string>();

    // 출제자 지정과 정답 문자열 제공을 위한 타이머
    private float timer;

    // 타이머 간격
    public float interval = 10.0f;

    // 출제자
    private Player questionMaster;

    public TMP_Text timerTxt;
    public UVDrawing pen;
    void Start()
    {
        players = new List<Player>();

        // 유저 리스트와 정답 테이블 초기화
        foreach (var player in PhotonNetwork.PlayerList)
        {
            players.Add(player);
        }


        timer = interval;
        Invoke("Choose", 1f);

    }
    void Choose()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AssignQuestionMaster();
            ProvideAnswer();
        }
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
                AssignQuestionMaster();
                ProvideAnswer();
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

    [PunRPC]
    void UpdateQuestionMaster(string playerName)
    {
        timer = interval;
        SetTimer(timer);
        pen.canDrawing = false;
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
            photonView.RPC("UpdateAnswer", questionMaster, answer);
        }
    }

    [PunRPC]
    void UpdateAnswer(string answer)
    {

        pen.canDrawing = true;
        Debug.Log("The answer is " + answer);

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
