using AbilitySystem;
using Photon.Pun;
using UnityEngine;

public class WinTileScript : MonoBehaviourPunCallbacks
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player") && !GameManager.instance.gameWon)
            GameManager.instance.photonView.RPC(nameof(GameManager.instance.DisplayWin), RpcTarget.All);
    }
}
