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
    public GameObject playerListContainer;
    public Button startGameButton;
    public Button[] classButtons;
    
    private List<RoomInfo> _roomList = new();
    private Dictionary<string, GameObject> _playerElements = new();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
    }
    private void Start()
    {
        mainScreen.SetActive(true);
        joinScreen.SetActive(false);
        lobbyScreen.SetActive(false);

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
        for (int i = playerListContainer.transform.childCount - 1; i >= 0; i--)
            Destroy(playerListContainer.transform.GetChild(i).gameObject);
        _playerElements = new();

        //playerListText.text = "";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            //playerListText.text += player.NickName + "\n";
            GameObject playerListElement = Instantiate(Resources.Load<GameObject>("UI/PlayerListElement"), playerListContainer.transform);
            playerListElement.GetComponentInChildren<TextMeshProUGUI>().text = player.NickName;
            _playerElements.Add(player.NickName, playerListElement.gameObject);
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
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("UpdateLobbyUI", RpcTarget.All);
    }
    public void OnLeaveLobbyButton()
    {
        PhotonNetwork.LeaveRoom();
        SetScreen(joinScreen);
        NetworkManager.instance.OnJoinLobby();
    }

    public void StartGameButton()
    {
        NetworkManager.instance.photonView.RPC("ChangeScene",
            RpcTarget.All, "MainScene");
    }
    public void MainMenuStart()
    {
        if (NetworkManager.instance.OnJoinLobby())
            SetScreen(joinScreen);
    }
    public void BackButton()
    {
        SetScreen(mainScreen);
    }
    public void QuitButton()
    {
        Application.Quit();
    }

    public void OnSelectClass(Sprite classSprite)
    {
        photonView.RPC(nameof(ChangeClassIcon),
            RpcTarget.All, PhotonNetwork.NickName, classSprite.name);
    }

    [PunRPC]
    public void ChangeClassIcon(string playerName, string spriteName)
    {
        _playerElements[playerName].GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"ClassUIImages/{spriteName}");
    }
}
