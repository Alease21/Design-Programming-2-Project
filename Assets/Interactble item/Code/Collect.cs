using UnityEngine;
using System.Collections;

public class Collectible : MonoBehaviour
{
    [Header("Collectible Settings")]
    public string collectibleName = "Item";
    //public int points = 10;
    public bool destroyOnCollect = true;
    public bool isPersistent = false; // For items that shouldn't be destroyed on collect

    [Header("Audio Settings")]
    public AudioClip collectSound;
    private AudioSource audioSource;

    [Header("Visual Effects")]
    public ParticleSystem collectEffect;
    public SpriteRenderer spriteRenderer;

    void Start()
    {
        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Cache sprite renderer for performance
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if player collided with this collectible
        if (other.CompareTag("Player"))
        {
            CollectItem();
        }
    }

    void CollectItem()
    {
        // Play sound if assigned
        if (collectSound != null)
            audioSource.PlayOneShot(collectSound);

        // Trigger visual effect if assigned
        if (collectEffect != null)
        {
            collectEffect.Play();
        }

        // Add points to player score (you'll need to implement this in your player script)
        //GameManager.Instance.AddScore(points);

        // Log collection
        Debug.Log($"Collected: {collectibleName}");

        // Destroy object if set and not persistent
        if (destroyOnCollect && !isPersistent)
            Destroy(gameObject);
        else if (isPersistent)
        {
            // Hide the object instead of destroying it
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
        }
    }
}
