using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MagicManager : MonoBehaviour
{
    public bool canAttack, attackCDBool, shooting;
    public float attackCooldown;
    public EnemyAttackSO playerAttack;
    public GameObject currentShootPoint, leftShootPoint, rightShootPoint;
    private Camera _mCam;
    public Vector2 targetArea;
    public static MagicManager magicInstance;
    public float cooldown, damage, repeats;

    public float damageMultiplier = 1f;

    private void Awake()
    {
        if (magicInstance == null)
        {
            magicInstance = this;
        }
        else if (magicInstance != this)
        {
            Destroy(this);
        }

        _mCam = Camera.main;
        canAttack = true;
        attackCDBool = true;
        shooting = false;
    }

    private void Update()
    {
        damage   = playerAttack.damage * damageMultiplier;
        cooldown = Mathf.Clamp(playerAttack.attackCooldown, 0.8f, 5f);
        repeats  = 1;

        ChangeShootPoint();

        if (Input.GetMouseButtonDown(0))
        {
            shooting = true;
        }
        if (Input.GetMouseButtonUp(0))
        {
            shooting = false;
        }

        if (canAttack && shooting)
        {
            targetArea = _mCam.ScreenToWorldPoint(Input.mousePosition);
            StartCoroutine(UseProjAttack(playerAttack, targetArea));
        }
    }

    private IEnumerator UseProjAttack(EnemyAttackSO currentAttack, Vector2 target)
    {
        Vector3 mousePosition = _mCam.ScreenToWorldPoint(Input.mousePosition);
        canAttack = false;

        for (int i = 0; i < repeats; i++)
        {
            if (attackCDBool)
            {
                attackCDBool = false;
                Instantiate(currentAttack.attackProj, currentShootPoint.transform.position, Quaternion.identity);
                yield return new WaitForSeconds(currentAttack.miniCooldown);
                attackCDBool = true;
            }
        }

        yield return new WaitForSeconds(cooldown);
        canAttack = true;
        yield return null;
    }

    private void ChangeShootPoint()
    {
        Vector3 mousePosition = _mCam.ScreenToWorldPoint(Input.mousePosition);

        if (transform.position.x < mousePosition.x)
        {
            currentShootPoint = leftShootPoint;
        }
        else
        {
            currentShootPoint = rightShootPoint;
        }
    }
}
