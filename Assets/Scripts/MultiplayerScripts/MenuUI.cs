using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

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
    public GameObject roomScreen;

    [Header("Main Screen")]
    public Button createRoomButton;
    public Button joinRoomButton;
    
    [Header("Lobby Screen")]
    public TextMeshProUGUI playerListText;
    public GameObject playerListContainer;
    public Button startGameButton;
    public Button[] classButtons;
    
    private List<RoomInfo> _roomList = new();
    private Dictionary<int, GameObject> _playerElements = new();

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        _roomList = roomList;
    }
    private void Start()
    {
        mainScreen.SetActive(true);
        joinScreen.SetActive(false);
        roomScreen.SetActive(false);

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
        roomScreen.SetActive(false);
        screen.SetActive(true);
    }

    public void OnCreateRoomButton()
    {
        string roomName = $"Room {_roomList.Count + 1}";
        NetworkManager.instance.CreateRoom(roomName);
    }
    public void OnJoinRoomButton(string roomName)
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

        ResetClassButtons();
        
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            /*int tempCounter = 0;
            while (_playerElements.ContainsKey(player.ActorNumber))
            {
                tempCounter++;
                player.NickName += $" ({tempCounter})";
            }*/

            GameObject playerListElement = Instantiate(Resources.Load<GameObject>("UI/PlayerListElement"), playerListContainer.transform);
            playerListElement.GetComponentInChildren<TextMeshProUGUI>().text = player.NickName;

            if (NetworkManager.instance.playersAndClass.ContainsKey(player.ActorNumber))
                playerListElement.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>($"ClassUIImages/{NetworkManager.instance.playersAndClass[player.ActorNumber]}");

            _playerElements.Add(player.ActorNumber, playerListElement.gameObject);
        }

        startGameButton.interactable = PhotonNetwork.IsMasterClient && (NetworkManager.instance.playersAndClass.Count == PhotonNetwork.PlayerList.Length);
    }

    public override void OnJoinedRoom()
    {
        SetScreen(roomScreen);
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
    public void OnLeaveRoomButton()
    {
        PhotonNetwork.CleanRpcBufferIfMine(photonView);
        NetworkManager.instance.LeaveRoom();
        SetScreen(joinScreen);
    }

    public void StartGameButton()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;

        NetworkManager.instance.photonView.RPC("ChangeScene", RpcTarget.All, "MainScene");
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
        photonView.RPC(nameof(MyClassIsSelected), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, classSprite.name);

        photonView.RPC(nameof(ChangeClassIcon), RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber, classSprite.name);

        ResetClassButtons();
    }
    public void ResetClassButtons()
    {
        string className = "";
        if (NetworkManager.instance.playersAndClass.ContainsKey(PhotonNetwork.LocalPlayer.ActorNumber))
            className = NetworkManager.instance.playersAndClass[PhotonNetwork.LocalPlayer.ActorNumber];

        foreach (var b in classButtons)
            b.interactable = true;

        if (className.Contains("Dwarf"))
            classButtons[0].interactable = false;
        else if (className.Contains("Duelist"))
            classButtons[1].interactable = false;
        else if (className.Contains("Plague"))
            classButtons[2].interactable = false;
    }
    [PunRPC]
    private void MyClassIsSelected(int id, string classSpriteName)
    {
        NetworkManager.instance.playersAndClass[id] = classSpriteName;

        if (NetworkManager.instance.playersAndClass.Count == PhotonNetwork.PlayerList.Length)
            if (PhotonNetwork.IsMasterClient)
                startGameButton.interactable = true;
    }
    [PunRPC]
    public void ChangeClassIcon(int id, string spriteName)
    {
        NetworkManager.instance.playersAndClass[id] = spriteName;
        UpdateLobbyUI();
    }
}
