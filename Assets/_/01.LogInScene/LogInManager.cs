using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEditor.PackageManager.Requests;


public class LogInManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwardField;
    [SerializeField] TMP_InputField userNameField;
    [SerializeField] Button connectBtn;
    [SerializeField] Button signhUpBtn;
    [SerializeField] GameObject LoadingBar;
    bool isConnected = false;
    private void Awake()
    {
       PhotonNetwork.ConnectUsingSettings();
    }
    // Start is called before the first frame update
    void Start()
    {

        connectBtn.onClick.AddListener(LogIn);
        signhUpBtn.onClick.AddListener(SignUp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void LogIn()
    {
        if(!isConnected)
        {
            Debug.Log("wait");
            return;
        }
        var request = new LoginWithEmailAddressRequest { Email = emailField.text, Password = passwardField.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }
    void SignUp()
    {
        var request = new RegisterPlayFabUserRequest { Email = emailField.text, Password = passwardField.text, Username = userNameField.text };
        PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("포톤접속");
        isConnected = true;
        LoadingBar.SetActive(false);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("등록성공");
        LogIn();
    }
    private void OnRegisterFailure(PlayFabError error)
    {
        Debug.LogError(error.GenerateErrorReport());
    }

    private void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Congratulations, you made your first successful API call!");

        // PlayFabId로 GetAccountInfo API 호출
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest
        {
            PlayFabId = result.PlayFabId
        },
        resultInfo =>
        {
            // API 호출 결과에서 UserName 가져오기
            PhotonNetwork.NickName = resultInfo.AccountInfo.Username;
            Utils.SceneChange(SceneNum.Lobby);
        },
        error =>
        {
            Debug.Log("Error getting account info: " + error.GenerateErrorReport());
        });
    }

    private void OnLoginFailure(PlayFabError error)
    {
        Debug.LogWarning("Something went wrong with your first API call.  :(");
        Debug.LogError("Here's some debug information:");
        Debug.LogError(error.GenerateErrorReport());
    }

}
