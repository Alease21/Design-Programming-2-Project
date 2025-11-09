using Photon.Pun;

public abstract class MultiplayerObjBase : MonoBehaviourPunCallbacks, IGUID
{
    protected string _GUID = "";
    public string GetGUID => _GUID;

    protected virtual void Awake() 
    {
        EvaluateGUID();
        //do guid register once implemented
    }

    // Set unique GUID
    public void EvaluateGUID()
    {
        if (_GUID == string.Empty)
            _GUID = System.Guid.NewGuid().ToString();
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
