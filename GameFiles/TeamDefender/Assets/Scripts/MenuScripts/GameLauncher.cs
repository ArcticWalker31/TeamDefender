using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Hashtable = ExitGames.Client.Photon.Hashtable;




public class GameLauncher : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text errorText;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] Transform roomListContent;
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject roomListItemPrefab;
    [SerializeField] GameObject playerListItemPrefab;
    [SerializeField] GameObject startGameButton;
    public GameObject valuables;
    public GameObject MenuButtons;


    bool joinedRoom;

    public int roomCode;



    public static GameLauncher Instance;

    void Awake()
    {
        Instance = this;
    }


    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }



    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        string path = Application.persistentDataPath + "/player.andy";

        if (File.Exists(path))
        {

            //PlayerNameManager.LoadPlayer();
            MenuManager.Instance.OpenMenu("MainMenu");
            valuables.SetActive(true);
            MenuButtons.SetActive(true);


            Debug.Log(path);
        }
        else
        {
            MenuManager.Instance.OpenMenu("username menu");
            valuables.SetActive(false);
            MenuButtons.SetActive(false);


        }
    }


    public void CreateRoom()
    {


        roomCode = Random.Range(1111111, 999999);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.CleanupCacheOnLeave = true;

        roomOptions.IsVisible = false;
        PhotonNetwork.CreateRoom(roomCode.ToString(), roomOptions, null);
        MenuManager.Instance.OpenMenu("Smaller Loading");
        joinedRoom = true;
    }

    public override void OnJoinedRoom()
    {

        roomNameText.text = PhotonNetwork.CurrentRoom.Name;

        if (joinedRoom == true)
        {
            MenuManager.Instance.OpenMenu("private room");
            joinedRoom = false;
        }

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }


        for (int i = 0; i < players.Length; i++)
        {
            //Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerList>().SetUp(players[i]);
        }

        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room Creation Failed: " + message;
        MenuManager.Instance.OpenMenu("error");

    }

    public void StartGame()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.Instance.OpenMenu("Smaller Loading");
    }

    public void JoinRoomThroughButton()
    {
        PhotonNetwork.JoinRoom(roomNameInputField.text);
        Debug.Log(roomNameInputField.text);
        joinedRoom = true;
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.Instance.OpenMenu("Smaller loading");
        joinedRoom = true;

    }

    public override void OnLeftRoom()
    {
        MenuManager.Instance.OpenMenu("MainMenu");

    }


    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform trans in roomListContent)
        {
            Destroy(trans.gameObject);
        }

        for (int i = 0; i < roomList.Count; i++)
        {
            if (roomList[i].RemovedFromList)
                continue;
            //Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomList>().SetUp(roomList[i]);
        }
    }


    public override void OnPlayerEnteredRoom(Player newplayer)
    {
        //Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerList>().SetUp(newplayer);
    }
}
