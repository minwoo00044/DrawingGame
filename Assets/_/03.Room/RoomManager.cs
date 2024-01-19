using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform txtP;
    [SerializeField] GameObject StartBtn;
    private TMP_Text[] userNames = new TMP_Text[8];
    private PhotonView _photonView;

    void Start()
    {
        userNames = txtP.GetComponentsInChildren<TMP_Text>();
        _photonView = GetComponent<PhotonView>(); // PhotonView 컴포넌트 가져오기
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            userNames[i].text = players[i].NickName;
        }
        if(PhotonNetwork.IsMasterClient)
        {
            StartBtn.GetComponent<Button>().onClick.AddListener(() => _photonView.RPC("StartGame", RpcTarget.All,SceneNum.Game1));
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            StartBtn.SetActive(false);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            userNames[i].text = players[i].NickName;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _photonView.RPC("RemoveLeftUserName", RpcTarget.All, otherPlayer.NickName);
    }

    [PunRPC]
    void RemoveLeftUserName(string playerName)
    {
        foreach (var name in userNames)
        {
            if (name.text == playerName)
            {
                name.text = string.Empty;
                break;
            }
        }
    }
    [PunRPC]
    void StartGame(SceneNum num)
    {
        Utils.SceneChange(num);
    }
}
