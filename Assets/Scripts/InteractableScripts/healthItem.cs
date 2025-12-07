using UnityEngine;

public class HealthItem : MonoBehaviour
{
    public int healAmount = 25;
    public AudioClip pickupSound;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UnitScript unit = other.GetComponent<UnitScript>();
            if (unit != null)
            {
                unit.ChangeHealth(healAmount, true);

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
