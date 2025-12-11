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

        PlayerMovement playerScript = playerObj.GetComponent<PlayerMovement>();

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

    //not currently used. was auto invoked on game end in example
    private void BackToMenu()
    {
        PhotonNetwork.LeaveRoom();

        NetworkManager.instance.ChangeScene("Menu");
    }
}
