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

    private void Start()
    {
        enterBtn.onClick.AddListener(() =>
        {
            photonView.RPC("RecieveChat", RpcTarget.All, PlayerNamer.Instance.myIdx, chatField.text);
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
                chatField.text = string.Empty;
            }
            else
            {
                chatField.Select();
                chatField.ActivateInputField();
            }
        }
    }
    [PunRPC]
    void RecieveChat(int idx, string sentence)
    {
        PlayerNamer.Instance.ActiveChat(idx, sentence);
    }
}
