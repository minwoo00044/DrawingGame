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
    private void Start()
    {
        requestBtn.onClick.AddListener(AddFriends);
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
        foreach (var friend in result.Friends)
        {

            Debug.Log($"Friend Username: {friend.Username}, DisplayName: {friend.TitleDisplayName}");
        }
    }

    private void OnError(PlayFabError error)
    {
        Debug.LogError($"Error getting friends list: {error.GenerateErrorReport()}");
    }
}
