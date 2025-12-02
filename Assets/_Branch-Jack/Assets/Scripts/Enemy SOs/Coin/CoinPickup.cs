using UnityEngine;

public class CoinPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Pickup");
            Destroy(this.gameObject);
        }
    }
}
