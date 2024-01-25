using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerNamer : MonoBehaviourPunCallbacks
{
    public static PlayerNamer Instance { get; private set; }
    public List<TMP_Text> playerInfoTxts = new List<TMP_Text>();
    public int myIdx;
    [SerializeField] ChatAnswerManager chatAnswerManager;
    [SerializeField] Transform infoMom;
    Player[] players;
    
    private void Start()
    {
        if(Instance == null)
            Instance = this;
        players = PhotonNetwork.PlayerList;
        chatAnswerManager = GetComponent<ChatAnswerManager>();
        photonView.RPC("SetUserName", RpcTarget.All);
    }
    [PunRPC]
    void SetUserName()
    {
        for(int i = 0; i < players.Length; i++)
        {
            playerInfoTxts[i].text = players[i].NickName;
            if (players[i].NickName == PhotonNetwork.NickName) 
            {
                myIdx = i;
            }
        }
    }
    public void ActiveChat(int idx, string sentence)
    {
        float time = 3;
        ChatUI target =  playerInfoTxts[idx].transform.parent.GetComponentInChildren<ChatUI>(true);
        target.SetTxt(sentence, time);
        target.gameObject.SetActive(true);
    }
}
