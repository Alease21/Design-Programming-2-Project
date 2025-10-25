using Photon.Pun;
using UnityEngine;

public class CamScripts : MonoBehaviourPunCallbacks, IPunObservable
{
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) { }
        //stream.SendNext(curFlagTime);
        else if (stream.IsReading) { }
        //curFlagTime = (float)stream.ReceiveNext();
    }
}
