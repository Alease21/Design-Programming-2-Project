using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudObjectScript : MonoBehaviour
{
    [Header("Cloud Settings")]
    public float duration;
    public int amountPerSecond = 1;
    public bool isFriendly;
    private Animator _animator;

    private readonly List<UnitScript> _unitsInside = new List<UnitScript>();

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    public IEnumerator CloudRoutine()
    {
        if (isFriendly)
        {
            _animator.Play("HealingExplosion");
        }
        else
            _animator.Play("PoisonExplosion");

        float elapsed = 0f;

        while (elapsed < duration)
        {
            for (int i = _unitsInside.Count - 1; i >= 0; i--)
            {
                var p = _unitsInside[i];
                if (p == null)
                {
                    _unitsInside.RemoveAt(i);
                    continue;
                }

                p.ChangeHealth(amountPerSecond, isFriendly);
            }

            yield return new WaitForSeconds(1f);

            elapsed++;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var unit = other.GetComponentInParent<UnitScript>();

        if (unit != null && !_unitsInside.Contains(unit) && CheckLayer(unit))
            _unitsInside.Add(unit);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var unit = other.GetComponentInParent<UnitScript>();

        if (unit != null && CheckLayer(unit))
            _unitsInside.Remove(unit);
    }
    public bool CheckLayer(UnitScript unit)
    {
        if ((unit.gameObject.layer == LayerMask.NameToLayer("Player") ||
            unit.gameObject.layer == LayerMask.NameToLayer("Friendly")) && isFriendly)
            return true;
        else if (unit.gameObject.layer == LayerMask.NameToLayer("Enemy") && !isFriendly)
            return true;
        return false;
    }
}
