using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using WFC;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public static NetworkManager instance;
    public bool useRandomSeed;
    public int dungeonSeed;
    public Dictionary<int, string> playersAndClass = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            PhotonView view = GetComponent<PhotonView>();
            if (view != null && view.ViewID == 0)//0 = invalid
                view.ViewID = 5; //over max (4 players)
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        //connect to the server
        PhotonNetwork.ConnectUsingSettings();
    }

    public void CreateRoom(string roomName)
    {
        PhotonNetwork.CreateRoom(roomName);
        if (useRandomSeed) 
            dungeonSeed = Random.Range(0, int.MaxValue);
    }

    public void JoinRoom(string roomName)
    {
        PhotonNetwork.JoinRoom(roomName);
    }
    public void LeaveRoom()
    {
        photonView.RPC(nameof(RemovePlayerFromClassDict), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.LeaveRoom();
    }
    [PunRPC]
    public void RemovePlayerFromClassDict(int id)
    {
        playersAndClass.Remove(id);
    }
    [PunRPC]
    public void ChangeScene(string sceneName)
    {
        PhotonNetwork.CleanRpcBufferIfMine(photonView);

        PhotonNetwork.LoadLevel(sceneName);
    }
    public bool OnJoinLobby()
    {
        return PhotonNetwork.JoinLobby();
    }
}
