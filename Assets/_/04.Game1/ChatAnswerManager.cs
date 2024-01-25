using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatAnswerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField ChatField;
    [SerializeField] Button enterBtn;

    private void Start()
    {
        enterBtn.onClick.AddListener(() => photonView.RPC("RecieveChat", RpcTarget.All, PlayerNamer.Instance.myIdx, ChatField.text)) ;
    }
    [PunRPC]
    void RecieveChat(int idx, string sentence)
    {
        PlayerNamer.Instance.ActiveChat(idx, sentence);
    }
}
