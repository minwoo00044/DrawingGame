using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.UI;
public class FriendManager : MonoBehaviour
{
    [SerializeField] TMP_InputField battleCodeInput;
    [SerializeField] Button requestBtn;
    [SerializeField] Transform contents;
    [SerializeField] GameObject friendsSlotPrefab;
    [SerializeField] int maxFriends;
    [SerializeField] Button togleBtn;
    [SerializeField] GameObject friendSet;

     private TMP_Text[] friends = new TMP_Text[100];
    private void Start()
    {
        togleBtn.onClick.AddListener(() => friendSet.SetActive(!friendSet.activeInHierarchy));
        for(int i = 0; i < maxFriends; i++)
        {
            GameObject instance = Instantiate(friendsSlotPrefab, contents);
            friends[i] = instance.GetComponentInChildren<TMP_Text>();
            instance.SetActive(false);
        }
        requestBtn.onClick.AddListener(AddFriends);
        GetFriendsList();
    }
    private void AddFriends()
    {
        if (battleCodeInput.text == string.Empty)
            return;
        var request = new AddFriendRequest {FriendUsername  = battleCodeInput.text};
        PlayFabClientAPI.AddFriend(request, OnAddFriendSuccess, OnAddFriendFail);
    }

    private void OnAddFriendSuccess(AddFriendResult result)
    {
        Debug.Log("친구 추가 성공");
        GetFriendsList();
    }

    private void OnAddFriendFail(PlayFabError error)
    {
        Debug.LogError("친구 추가 실패: " + error.GenerateErrorReport());
    }

    public void GetFriendsList()
    {
        var request = new GetFriendsListRequest();
        PlayFabClientAPI.GetFriendsList(request, OnGetFriendsListSuccess, OnError);
    }

    private void OnGetFriendsListSuccess(GetFriendsListResult result)
    {
        int i = 0;
        foreach (var friend in result.Friends)
        {
            GameObject targetG0 = friends[i].transform.parent.gameObject;
            if(!targetG0.gameObject.activeInHierarchy)
            {
                targetG0.gameObject.SetActive(true);
            }
            friends[i].text = $"{friend.TitleDisplayName}#{friend.Username}";
            i++;
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError($"Error getting friends list: {error.GenerateErrorReport()}");
    }
}
