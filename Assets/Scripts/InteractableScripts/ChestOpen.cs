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
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                isOpen = true;
                spriteRen.sprite = openChest;
                Debug.Log("chest open");
            }
        }
    }
}
