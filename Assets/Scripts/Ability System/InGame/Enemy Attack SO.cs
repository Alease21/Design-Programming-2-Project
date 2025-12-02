using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "EnemyAttack")]
public class EnemyAttackSO : ScriptableObject
{
    public float damage;
    public float attackRange;
    public float attackCooldown;
    public GameObject attackProj;
    public float miniCooldown;
    public float projSpeed;
    public int repeats;
    public bool isProjectile;
    public float range;
    public Vector3 rotation;
}
