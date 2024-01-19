using Photon.Pun;
using Photon.Realtime;
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


    }
    void CreateRoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 8;
        PhotonNetwork.CreateRoom(PhotonNetwork.NickName + "의 방", roomOptions);
        
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("룸 생성 실패");
    }
    public override void OnJoinedRoom()
    {
        Utils.SceneChange(SceneNum.Room);
    }
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.LogWarning(1111);
        GameObject tempRoom = null;
        foreach(var room in roomList)
        {
            if(room.RemovedFromList)
            {
                roomDict.TryGetValue(room.Name, out tempRoom);
                Destroy(tempRoom);
                roomDict.Remove(room.Name);
            }
            else
            {
                //처음 생성된 룸
                if(!roomDict.ContainsKey(room.Name))
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
