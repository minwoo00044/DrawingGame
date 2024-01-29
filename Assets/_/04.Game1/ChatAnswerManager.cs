using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatAnswerManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField chatField;
    [SerializeField] Button enterBtn;
    GameManager1 gameManager1;
    PlayerNamer namer;
    private void Start()
    {
        namer = FindObjectOfType<PlayerNamer>();
        gameManager1 = FindAnyObjectByType<GameManager1>();
        enterBtn.onClick.AddListener(() =>
        {
            photonView.RPC("RecieveChat", RpcTarget.All, PlayerNamer.Instance.myIdx, chatField.text);
            if(ChatIsAnswer(chatField.text))
            {
                photonView.RPC("Correct", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, namer.myIdx);
            }
            chatField.text = string.Empty;
        });


    }
    private void Update()
    {
        if (Input.GetButtonDown("Submit"))
        {
            if(chatField.text != string.Empty) 
            {
                photonView.RPC("RecieveChat", RpcTarget.All, PlayerNamer.Instance.myIdx, chatField.text);
                if (ChatIsAnswer(chatField.text))
                {
                    photonView.RPC("Correct", RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, namer.myIdx);
                }
                chatField.text = string.Empty;
            }
            else
            {
                chatField.Select();
                chatField.ActivateInputField();
            }
        }
    }
    bool ChatIsAnswer(string chat)
    {
        if (chat == GameManager1.answer)
            return true;
        else
            return false;
    }
    [PunRPC]
    void RecieveChat(int idx, string sentence)
    {
        PlayerNamer.Instance.ActiveChat(idx, sentence);
    }
}
