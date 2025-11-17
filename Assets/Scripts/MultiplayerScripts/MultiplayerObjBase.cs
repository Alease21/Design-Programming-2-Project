using Photon.Pun;
using System.Numerics;

public abstract class MultiplayerObjBase : MonoBehaviourPunCallbacks, IGUID
{
    protected string _GUID = "";
    public string GetGUID => _GUID;

    protected virtual void Awake() 
    {
        //EvaluateGUID();
        //do guid register once implemented
    }

    // Set unique GUID
    [PunRPC]
    public void EvaluateGUID(Vector2 objWorldPos)
    {
        UnityEngine.Random.InitState(NetworkManager.instance.dungeonSeed);

        // **add check to ensure no duplicate guids are created
        if (_GUID == string.Empty)
            _GUID = (UnityEngine.Random.Range(0, int.MaxValue) + int.Parse($"{objWorldPos.X}{objWorldPos.Y}")).ToString();
    }

    // Destroy or disable object of matching guid for every player
    [PunRPC]
    public void OnMultiplayerObjDestroy(string guid, bool shouldDestroy = false)
    {
        if (guid != _GUID) return;

        //remove guid from registry once implemented?

        if (shouldDestroy)
            Destroy(this.gameObject);
        else
            this.gameObject.SetActive(false);
    }
}
