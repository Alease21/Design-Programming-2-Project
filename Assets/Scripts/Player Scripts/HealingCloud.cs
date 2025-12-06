using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HealingCloud : MonoBehaviour
{
    [Header("Healing Settings")]
    public float duration = 7f;
    public float healPerSecond = 1f;

    private readonly List<PlayerHealthManager> _playersInside = new List<PlayerHealthManager>();

    private void Reset()
    {
        var col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void Start()
    {
        Debug.Log("HealingCloud (3D) started on " + gameObject.name);
        StartCoroutine(CloudLifeRoutine());
    }

    private IEnumerator CloudLifeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float healAmount = healPerSecond * Time.deltaTime;

            for (int i = _playersInside.Count - 1; i >= 0; i--)
            {
                var p = _playersInside[i];
                if (p == null)
                {
                    _playersInside.RemoveAt(i);
                    continue;
                }

                p.Heal(healAmount);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealthManager>();

        if (health != null && !_playersInside.Contains(health))
        {
            Debug.Log(">>> Player entered heal cloud: " + health.gameObject.name);
            _playersInside.Add(health);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var health = other.GetComponentInParent<PlayerHealthManager>();

        if (health != null)
        {
            Debug.Log("<<< Player left heal cloud: " + health.gameObject.name);
            _playersInside.Remove(health);
        }
    }
}
