using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 25;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.Heal(healAmount);

                // Play pickup sound if assigned
                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
                }

                // Destroy the health item
                Destroy(gameObject);
            }
        }
    }
}
