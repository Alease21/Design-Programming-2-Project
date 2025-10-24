using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy")]
public class EnemySO : ScriptableObject
{
    public new string name;
    public Sprite charSprite;
    public float speed;
    public float hp;
    public float defense;
    public bool isBoss;
    public int enemyTier;
    public int weight;
}
