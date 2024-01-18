using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LogInManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField nickNameField;
    [SerializeField] Button connectBtn;
    bool isConnected = false;
    private void Awake()
    {
       PhotonNetwork.ConnectUsingSettings();
    }
    // Start is called before the first frame update
    void Start()
    {
        connectBtn.onClick.AddListener(Connect);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void Connect()
    {
        if(!isConnected)
        {
            Debug.Log("wait");
            return;
        }
        PhotonNetwork.NickName = nickNameField.text;
        SceneManager.LoadScene("LobbyScene");
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("Æ÷ÅæÁ¢¼Ó");
        isConnected = true;
    }
}
