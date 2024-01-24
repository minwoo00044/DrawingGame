using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class GameManager1 : MonoBehaviourPunCallbacks
{
    // ���� ����Ʈ�� ���� ���̺�
    private List<Player> players;
    public List<string> answerTable = new List<string>();

    // ������ ������ ���� ���ڿ� ������ ���� Ÿ�̸�
    private float timer;

    // Ÿ�̸� ����
    public float interval = 10.0f;

    // ������
    private Player questionMaster;

    public TMP_Text timerTxt;
    public UVDrawing pen;
    void Start()
    {
        players = new List<Player>();

        // ���� ����Ʈ�� ���� ���̺� �ʱ�ȭ
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
        // ������ Ŭ���̾�Ʈ������ ������ ������ ���� ���� ���� ����

        timer -= Time.deltaTime;
        SetTimer(timer);
        if (timer <= 0)
        {
            timer = interval;
            if (PhotonNetwork.IsMasterClient)
            {
                AssignQuestionMaster();
                ProvideAnswer();
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

            // �ش� ������ ����Ʈ���� ����
            answerTable.RemoveAt(index);

            // ������ �����ڿ��Ը� ����
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
    void SyncDrawing(Vector2 uv, Color color, int brushSize)
    {
        // ���� �׸� ������ �׸� �׸���
        pen.DrawOnTexture(uv, color, brushSize);
    }
}
