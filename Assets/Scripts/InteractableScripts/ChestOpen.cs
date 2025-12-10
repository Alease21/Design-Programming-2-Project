using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    public Sprite openChest, closedChest;
    public bool isOpen;
    public SpriteRenderer spriteRen;
    public bool playerInRange = false;

    private void Awake()
    {
        spriteRen = GetComponent<SpriteRenderer>();
        spriteRen.sprite = closedChest;
        isOpen = false;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (playerInRange)
            {
                isOpen = true;
                spriteRen.sprite = openChest;
                Debug.Log("chest open");
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerInRange = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
            playerInRange = false;
    }
}
