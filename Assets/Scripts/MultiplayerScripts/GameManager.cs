using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Linq;

public class GameManager : MonoBehaviourPunCallbacks
{
    //[Header("Game Stuff")]

    [Header("Players")]
    public string playerPrefabLoc;

    public Transform[] spawnPoints;
    public PlayerMovement[] players;
    private int _playersInGame;

    public static GameManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        players = new PlayerMovement[PhotonNetwork.PlayerList.Length];
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
        GameObject playerObj = PhotonNetwork.Instantiate(playerPrefabLoc, 
            spawnPoints[Random.Range(0, spawnPoints.Length)].position, 
            Quaternion.identity);

        PlayerMovement playerScript = playerObj.GetComponent<PlayerMovement>();

        playerScript.photonView.RPC("Initialize", RpcTarget.All, PhotonNetwork.LocalPlayer);
    }

    public PlayerMovement GetPlayer(int playerID)
    {
        return players.First(p => p.id == playerID);
    }
    public PlayerMovement GetPlayer(GameObject playerObj)
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
