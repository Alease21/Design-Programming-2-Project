using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

public class MenuUI : MonoBehaviourPunCallbacks
{
    public static MenuUI instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    [Header ("Screens")]
    public GameObject mainScreen;
    public GameObject joinScreen;
    public GameObject lobbyScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;

    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public Button startGameButton;

    private List<RoomInfo> _roomList;

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
    }
    private void Start()
    {
        createRoomButton.interactable = false;
        joinRoomButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        createRoomButton.interactable = true;
        joinRoomButton.interactable = true;
    }

    private void SetScreen(GameObject screen)
    {
        mainScreen.SetActive(false);
        joinScreen.SetActive(false);
        lobbyScreen.SetActive(false);
        screen.SetActive(true);
    }

    public void OnCreateRoomButton(/*TMP_InputField roomNameInput*/)
    {
        string roomName = $"Room {_roomList.Count + 1}";
        NetworkManager.instance.CreateRoom(roomName /*roomNameInput.text*/);
    }

    public void OnJoinRoomButton(string roomName/*TMP_InputField roomNameInput*/)
    {
        NetworkManager.instance.JoinRoom(roomName);
    }

    public void OnPlayerNameUpdate(TMP_InputField playerNameInput)
    {
        PhotonNetwork.NickName = playerNameInput.text;
    }

    [PunRPC]
    public void UpdateLobbyUI()
    {
        playerListText.text = "";
        foreach(Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }

        if(PhotonNetwork.IsMasterClient)
            startGameButton.interactable = true;
        else
            startGameButton.interactable = false;
    }

    public override void OnJoinedRoom()
    {
        SetScreen(lobbyScreen);
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateLobbyUI();
    }

    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(joinScreen);
    }

    public void StartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene",
            RpcTarget.All, "MainScene");
    }
    public void MainMenuStart()
    {

    }
    public void BackButton()
    {
        SetScreen(mainScreen);
    }
}
