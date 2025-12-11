using AbilitySystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{
    public LayerMask layerMask;
    public bool isFinished = false;
    public List<GameObject> target = new();
    public float range;

    private void Awake()
    {
        Invoke("ProjectileFinished", 5f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            ProjectileFinished();
        }
        else if (other != null && ((1 << other.gameObject.layer) & layerMask) != 0)
        {
            target.Add(other.gameObject);
            ProjectileFinished();
        }
    }

    public void ProjectileFinished()
    {
        if (target.Count == 0)
        {
            GameObject emptyTar = new GameObject();
            emptyTar.transform.position = transform.position;
            target.Add(emptyTar);
        }

        isFinished = true;
    }
}