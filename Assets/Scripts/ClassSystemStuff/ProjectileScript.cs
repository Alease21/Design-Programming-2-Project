using MagicSystem;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public SpellController caster;
    public LayerMask layerMask = 0;
    public bool isFinished = false;
    public IEnumerable<GameObject> target;
    public float range;

    private void Awake()
    {
        Invoke("ProjectileFinished", 5f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other != null && other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            target = new List<GameObject>() { other.gameObject };
            ProjectileFinished();
        }
    }

    private void ProjectileFinished()
    {
        isFinished = true;
    }
}