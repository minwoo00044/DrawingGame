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
using UnityEngine.EventSystems;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class LogInManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField emailField;
    [SerializeField] TMP_InputField passwordField;
    [SerializeField] TMP_InputField userNameField;
    [SerializeField] Button connectBtn;
    [SerializeField] Button signhUpBtn;
    [SerializeField] GameObject LoadingBar;
    [SerializeField] TMP_Text logTxt;
    bool isConnected = false;
    private void Awake()
    {
        GetComponent<PlayFabAuthenticator>().AuthenticateWithPlayFab();
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
        if(Input.GetKeyDown(KeyCode.Tab))
        {
            ChangeTab();
        }
    }
    private void ChangeTab()
    {
        if (emailField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(passwordField.gameObject, null);
            passwordField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
        else if (passwordField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(userNameField.gameObject, null);
            userNameField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
        else if (userNameField.isFocused)
        {
            EventSystem.current.SetSelectedGameObject(emailField.gameObject, null);
            emailField.OnPointerClick(new PointerEventData(EventSystem.current));
        }
    }
    void LogIn()
    {
        if(!isConnected)
        {
            Debug.Log("wait");
            return;
        }
        var request = new LoginWithEmailAddressRequest { Email = emailField.text, Password = passwordField.text };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnLoginSuccess, OnLoginFailure);

    }
    private void SignUp()
    {
        GenerateUniqueUsernameAndSignUp();
    }

    private void GenerateUniqueUsernameAndSignUp()
    {
        var randomUsername = GenerateRandomUsername();
        CheckUsernameExists(randomUsername, exists =>
        {
            if (!exists)
            {
                var request = new RegisterPlayFabUserRequest
                {
                    Email = emailField.text,
                    Password = passwordField.text,
                    Username = randomUsername,
                    DisplayName = userNameField.text
                };
                PlayFabClientAPI.RegisterPlayFabUser(request, OnRegisterSuccess, OnRegisterFailure);
            }
            else
            {
                // If username exists, try again.
                GenerateUniqueUsernameAndSignUp();
            }
        });
    }

    private string GenerateRandomUsername()
    {
        var randomNumber = Random.Range(0, 1000000); // Generates a random number between 0 and 999999.
        return randomNumber.ToString("D6"); // Formats the number to a 6-digit string, padding with zeros if necessary.
    }

    private void CheckUsernameExists(string username, Action<bool> callback)
    {
        var request = new GetAccountInfoRequest { Username = username };
        PlayFabClientAPI.GetAccountInfo(request,
            result => callback(false), // If the request succeeds, username does not exist.
            error =>
            {
                if (error.Error == PlayFabErrorCode.AccountNotFound)
                {
                    callback(false); // If account not found, username does not exist.
                }
                else
                {
                    callback(true); // If other error, assume username might exist to be safe.
                }
            });
    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("��������");
        isConnected = true;
        LoadingBar.SetActive(false);
    }
    private void OnRegisterSuccess(RegisterPlayFabUserResult result)
    {
        Debug.Log("��ϼ���");
        LogIn();
    }
    private void OnRegisterFailure(PlayFabError error)
    {
        LogOn("ȸ������ ���� �̤�");
        Debug.LogError(error.GenerateErrorReport());
    }
    private void LogOn(string log)
    {
        logTxt.gameObject.SetActive(true);
        logTxt.text = log;
        Utils.FadeOut(logTxt.GetComponent<TMP_Text>(), 2f);
    }

    private void OnLoginSuccess(LoginResult result)
    {
        PlayerPrefs.SetString("PlayFabId", result.PlayFabId);
        // �α��� ���� �� GetAccountInfo API ȣ��
        PlayFabClientAPI.GetAccountInfo(new GetAccountInfoRequest()
        {
            PlayFabId = result.PlayFabId
        },
        resultInfo =>
        {
            if (resultInfo.AccountInfo != null)
            {
                // Username ��ȸ
                string username = resultInfo.AccountInfo.Username != null ? resultInfo.AccountInfo.Username : "No Username";
                // DisplayName ��ȸ (DisplayName�� �������� �ʾ����� Username�� ���)
                string displayName = resultInfo.AccountInfo.TitleInfo != null && resultInfo.AccountInfo.TitleInfo.DisplayName != null ? resultInfo.AccountInfo.TitleInfo.DisplayName : username;

                // DisplayName�� PhotonNetwork.NickName�� ����
                PhotonNetwork.NickName = displayName+"#"+username;
                Utils.SceneChange(SceneNum.Lobby);
            }
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
        LogOn("�α��� ���� �̤�");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        LoadingBar.GetComponent<TMP_Text>().text = "�������ߤ̤�";
        PhotonNetwork.ConnectUsingSettings();
    }

}
