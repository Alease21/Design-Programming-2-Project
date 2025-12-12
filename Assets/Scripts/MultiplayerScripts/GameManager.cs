using UnityEngine;
using System.Collections.Generic;
using Photon.Pun;
using System.Linq;
using WFC;

public class GameManager : MonoBehaviourPunCallbacks
{
    //[Header("Game Stuff")]

    [Header("Players")]
    public string playerPrefabLoc;

    public Transform[] spawnPoints = new Transform[4];
    public PlayerMultiplayerIdScript[] players;
    public List<EnemyFSM> enemies = new();
    private int _playersInGame;

    public GameObject winDisplay;
    public float winTextDuration = 5f;
    public bool gameWon = false;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DungeonCreator.instance.WFCFinished += OnWFCDone;
    }

    private void OnWFCDone()
    {
        players = new PlayerMultiplayerIdScript[PhotonNetwork.PlayerList.Length];
        photonView.RPC("ImInGame", RpcTarget.All);
    }
    [PunRPC]
    private void ImInGame()
    {
        _playersInGame++;
        if (_playersInGame == PhotonNetwork.PlayerList.Length)
            SpawnPlayer();
    }
    
    private void SpawnPlayer()
    {
        int playerIndex = 0;
        for (int i = 0; i < PhotonNetwork.PlayerList.Length; i++)
            if (PhotonNetwork.PlayerList[i] == PhotonNetwork.LocalPlayer)
            {
                playerIndex = i;
                break;
            }

        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLoc, spawnPoints[playerIndex].position, Quaternion.identity);
        playerObj.name = $"Player - {playerIndex}";
        PlayerMultiplayerIdScript playerScript = playerObj.GetComponent<PlayerMultiplayerIdScript>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerMultiplayerIdScript GetPlayer(int playerID)
    {
        return players.First(p => p.id == playerID);
    }
    public PlayerMultiplayerIdScript GetPlayer(GameObject playerObj)
    {
        return players.First(p => p.gameObject == playerObj);
    }

    public void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();

        NetworkManager.instance.playersAndClass = new();
        NetworkManager.instance.ChangeScene("Menu");
    }

    [PunRPC]
    public void DisplayWin()
    {
        gameWon = true;
        winDisplay.SetActive(true);
        Invoke(nameof(BackToMenu), winTextDuration);
    }
}
