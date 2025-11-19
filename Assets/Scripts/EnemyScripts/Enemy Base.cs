using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static MagicManager;

public class EnemyBase : MultiplayerObjBase, IEnemy
{
    public EnemySO baseStats;
    public List<EnemyAttackSO> attacks;
    public EnemyAttackSO currentAttack;
    public GameObject targetPlayer;
    public UnityEvent OnDeath;
    public bool canAttack = true;
    public bool attackCooldown = true;
    public float curHP;
    public int curTier;
    public SpriteRenderer spriteRenderer;
    public GameObject coins;

    protected override void Awake()
    {
        base.Awake();// call base Awake from MultiplayerGameObject for setting GUID & maybe other stuff eventually
        
        curHP = baseStats.hp;
        targetPlayer = GameObject.FindGameObjectWithTag("Player");
        if (baseStats.charSprite != null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = baseStats.charSprite;
        }
    }

    private void Update()
    {
        if(spriteRenderer.sprite != baseStats.charSprite)
        {
            spriteRenderer.sprite = baseStats.charSprite;
        }
        if (currentAttack == null)
        {
            int curAttack = Random.Range(0,attacks.Count);
            currentAttack = attacks[curAttack];
        }
        if (targetPlayer != null)
        {
            if (Vector2.Distance(transform.position, targetPlayer.transform.position) >= currentAttack.attackRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, targetPlayer.transform.position, baseStats.speed * Time.deltaTime);
            }
            else if (canAttack)
            {
                if (currentAttack.isProjectile)
                {
                    StartCoroutine(UseProjAttack(currentAttack, targetPlayer));
                }
            }
        }
        else
        {
            targetPlayer = GameObject.FindGameObjectWithTag("Player");
        }

        //LookAtPlayer();
    }

    private void LookAtPlayer()
    {
        if (transform.position.x < targetPlayer.transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
        }
    }

    private IEnumerator UseProjAttack(EnemyAttackSO currentAttack, GameObject targetPlayer)
    {
        canAttack = false;
        for (int i = 0; i < currentAttack.repeats; i++)
        {
            if (attackCooldown)
            {
                attackCooldown = false;
                Instantiate(currentAttack.attackProj, transform);
                yield return new WaitForSeconds(currentAttack.miniCooldown);
                attackCooldown = true;
            }

        }
        yield return new WaitForSeconds(currentAttack.attackCooldown);
        //Debug.Log("Can Attack");
        canAttack = true;
        currentAttack = null;
        yield return null;
    }

    public void TakeDamage(float damage)
    {
        float damageTaken = Mathf.Clamp(damage - (baseStats.defense * baseStats.enemyTier),(damage/10), Mathf.Infinity);
        curHP = curHP - damageTaken;

        if (curHP <= 0)
        {
            OnDeath?.Invoke();
            GameObject coinDrop = Instantiate(coins, transform.position, transform.rotation);
            coinDrop.transform.parent = null;

            Destroy(this.gameObject);
            //photonView.RPC("OnMultiplayerObjDestroy", Photon.Pun.RpcTarget.All);
        }
    }
}
