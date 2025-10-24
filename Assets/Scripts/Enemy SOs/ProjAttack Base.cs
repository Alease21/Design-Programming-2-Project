using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class ProjAttackBase : MonoBehaviour
{
    public Vector2 targetDirection;
    public Vector2 startPosition;
    public EnemyAttackSO curStats;
    public SpriteRenderer spriteRenderer;
    public GameObject targetPlayer;
    public Transform spriteChild;
    private void Awake()
    {
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        startPosition = transform.position;
        targetDirection = targetPlayer.transform.position - transform.position;
        targetDirection.Normalize();
        spriteChild = transform.GetChild(0);
        Vector3 direction = targetPlayer.transform.position - transform.position;
        float rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        spriteChild.rotation = Quaternion.Euler(0, 0, rotation+180);
        /*if (transform.position.x < targetPlayer.transform.position.x)
        {
            spriteRenderer.flipY = true;
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipY = false;   
            spriteRenderer.flipX = true ;
        }*/
    }

    private void Update()
    {
        transform.Translate(transform.rotation * targetDirection * curStats.projSpeed * Time.deltaTime);
        if (Vector2.Distance(startPosition, transform.position) > curStats.range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Works whether the collider is on the root or a child of the player
        var player = collision.GetComponent<IPlayer>() ?? collision.GetComponentInParent<IPlayer>();
        if (player != null)
        {
            player.TakeDamage(curStats != null ? curStats.damage : 1f);
            Destroy(gameObject);
        }
    }

}
