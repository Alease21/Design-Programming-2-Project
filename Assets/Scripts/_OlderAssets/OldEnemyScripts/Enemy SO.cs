using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public new string name;
    public Sprite sprite;
    public float speed;
    public float maxHealth;
    public float defense;
    public float idleDuration;
    public float roamRadius;
    public EnemyAttackSO attackSO;
    //public bool isBoss;
    public int enemyTier;
    //public int weight;
}
