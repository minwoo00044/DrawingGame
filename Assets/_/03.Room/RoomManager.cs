using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
class GameBtn
{
    public Button btn;
    public SceneNum sceneNum;
}
public class RoomManager : MonoBehaviourPunCallbacks
{
    [SerializeField] Transform txtP;
    [SerializeField] GameObject masterBtns;
    [SerializeField] GameObject StartBtn;
    [SerializeField] Transform highlLight;

    [SerializeField] List<GameBtn> btns = new List<GameBtn>();
    private TMP_Text[] userNames = new TMP_Text[8];
    private PhotonView _photonView;
    [SerializeField]private SceneNum _sceneNum = SceneNum.Game1;


    void Start()
    {
        userNames = txtP.GetComponentsInChildren<TMP_Text>();
        _photonView = GetComponent<PhotonView>(); // PhotonView 컴포넌트 가져오기
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            userNames[i].text = players[i].NickName;
        }
        if(PhotonNetwork.IsMasterClient)
        {
            foreach(var item in btns)
            {
                item.btn.onClick.AddListener(()=>GameChange(item.btn.transform, item.sceneNum));
            }
            StartBtn.GetComponent<Button>().onClick.AddListener(() => _photonView.RPC("StartGame", RpcTarget.All, _sceneNum));
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            masterBtns.SetActive(false);
        }
    }
    public void GameChange(Transform parent, SceneNum num)
    {
        if(!highlLight.gameObject.activeInHierarchy)
            highlLight.gameObject.SetActive(true);
        highlLight.SetParent(parent);
        highlLight.transform.localPosition = new Vector3(-50, 50, 0);
        _sceneNum = num;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++)
        {
            userNames[i].text = players[i].NickName;
        }
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        _photonView.RPC("RemoveLeftUserName", RpcTarget.All, otherPlayer.NickName);
    }

    [PunRPC]
    void RemoveLeftUserName(string playerName)
    {
        foreach (var name in userNames)
        {
            if (name.text == playerName)
            {
                name.text = string.Empty;
                break;
            }
        }
    }
    [PunRPC]
    void StartGame(SceneNum num)
    {
        Utils.SceneChange(num);
    }
}
