using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class PlayerMultiplayerIdScript : MonoBehaviourPunCallbacks
{
    public int id;
    public Player photonPlayer;

    [PunRPC]
    public void Initialize(Player player)
    {
        photonPlayer = player;
        id = player.ActorNumber;
        GameManager.instance.players[id - 1] = this;

        GetComponent<PlayerMovement>().InitializeRB();
    }
}
