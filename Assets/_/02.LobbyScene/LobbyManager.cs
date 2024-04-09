using Photon.Pun;
using Photon.Realtime;
using PlayFab.ClientModels;
using PlayFab;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_Text nameTxt;
    [SerializeField] Button createRoomBtn;
    [SerializeField] GameObject roomPrefab;
    [SerializeField] Transform scrollContent;
    [SerializeField] TMP_Text myRankPoint;
    private Dictionary<string, GameObject> roomDict = new Dictionary<string, GameObject>();
    // Start is called before the first frame update

    private void Awake()
    {
    }
    void Start()
    {

        PhotonNetwork.JoinLobby();

        createRoomBtn.onClick.AddListener(CreateRoom);
    }

    // Update is called once per frame
    void Update()
    {

    }
    public override void OnJoinedLobby()
    {
        Debug.Log("로비 입장");
        nameTxt.text = PhotonNetwork.NickName;
        GetRankPoint();
    }

    private void GetRankPoint()
    {
        string playFabId = PlayerPrefs.GetString("PlayFabId"); // PlayerPrefs에서 PlayFabId 불러오기
        Debug.Log(playFabId);
        PlayFabClientAPI.GetUserData(new GetUserDataRequest()
        {
            PlayFabId = playFabId, // Your PlayFabId
            Keys = new List<string>() { "RankPoint" }
        }, result =>
        {
            if (result.Data != null)
            {
                if (result.Data.ContainsKey("RankPoint"))
                {
                    Debug.Log("RankPoint: " + result.Data["RankPoint"].Value);
                    myRankPoint.text = result.Data["RankPoint"].Value;
                }
                else
                    myRankPoint.text = "첫 게임을 경험해 보세요!";


            }
        }, error =>
        {
            myRankPoint.text = "점수 불러오기 실패";
        });
    }

    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;
        Debug.Log(PhotonNetwork.NickName);

        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "의 방", roomOptions);
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }
    public override void OnJoinedRoom()
    {
        Utils.SceneChange(SceneNum.Room);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        GameObject tempRoom = null;
        foreach (var room in roomList)
        {
            if (room.RemovedFromList)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                //처음 생성된 룸
                if (!roomDict.ContainsKey(room.Name))
                {
                    GameObject instance = Instantiate(roomPrefab, scrollContent);
                    instance.GetComponentInChildren<TMP_Text>().text = room.Name;
                    instance.GetComponent<Button>().onClick.AddListener
                        (() => PhotonNetwork.JoinRoom(room.Name));
                    roomDict.Add(room.Name, instance);
                }
                else
                {
                    roomDict.TryGetValue(room.Name, out tempRoom);
                    tempRoom.GetComponent<TMP_Text>().text = room.Name;
                    tempRoom.GetComponent<Button>().onClick.AddListener
                        (() => PhotonNetwork.JoinRoom(room.Name));
                }
            }
        }

    }
}
