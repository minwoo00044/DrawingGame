using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using UnityEngine.SceneManagement;
using Photon.Pun;

public class WhisperManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    private void Awake()
    {
        // 현재 GameObject를 씬 전환 시에도 파괴되지 않도록 설정
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        this.chatClient = new ChatClient(this);
        StartCoroutine(CloudScriptHelper.GetPhotonAppIdFromCloudScript((photonAppId) => {
            Debug.LogWarning(photonAppId);
            this.chatClient.Connect(photonAppId, PhotonNetwork.AppVersion, new AuthenticationValues(PhotonNetwork.NickName));
        }));
    }
    #region 씬넘길때 카메라찾기
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UpdateCanvasCamera();
    }
    private void UpdateCanvasCamera()
    {
        Canvas canvas = GetComponent<Canvas>(); 
        if (canvas != null)
        {
            canvas.worldCamera = Camera.main;
        }
    }
    #endregion
    void Update()
    {
        // ChatClient의 서비스를 지속적으로 업데이트
        if (this.chatClient != null)
        {
            this.chatClient.Service();
        }
    }

    public void SendWhisperMessage(string targetUserName, string message)
    {
        chatClient.SendPrivateMessage(targetUserName, message);
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        Debug.Log("DebugReturn: " + message);
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log("OnChatStateChange: " + state);
    }

    public void OnDisconnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        throw new System.NotImplementedException();
    }

    public void OnUnsubscribed(string[] channels)
    {
        throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }
}
