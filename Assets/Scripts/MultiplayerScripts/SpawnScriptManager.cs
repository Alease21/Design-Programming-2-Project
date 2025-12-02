using UnityEngine;

public class SpawnScriptManager : MonoBehaviour
{
    public GameObject networkManager;

    private void Awake()
    {
        if (FindAnyObjectByType<NetworkManager>() == null)
        {
            Instantiate(networkManager);
        }
    }
}
