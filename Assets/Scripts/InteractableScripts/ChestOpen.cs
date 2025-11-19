using UnityEngine;

public class ChestOpen : MonoBehaviour
{
    public Sprite openChest, closedChest;
    public bool isOpen;
    public SpriteRenderer spriteRen;
    private void Awake()
    {
        spriteRen = GetComponent<SpriteRenderer>();
        spriteRen.sprite = closedChest;
        isOpen = false;
    }
    private void OnTriggerStay(Collider collision)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isOpen = true;
                spriteRen.sprite = openChest;
                Debug.Log("chest open");
            }
        }
    }
}
