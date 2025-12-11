using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class PlayerReviveScript : MonoBehaviour
{
    private CircleCollider2D _reviveTrigger;
    [SerializeField] private float _reviveRadius = 2f;
    private List<PlayerDownedScript> _downedUnitsInRange = new();
    private PlayerDownedScript _reviveTarget;
    [SerializeField] private float _reviveChannelTime;
    [SerializeField] private bool _isReviving = false;
    [SerializeField] private float _reviveTimer = 0f;

    private void Awake()
    {
        _reviveTrigger = GetComponent<CircleCollider2D>();
        _reviveTrigger.radius = _reviveRadius;
        _reviveTrigger.enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerDownedScript>(out PlayerDownedScript tar) && tar.IsDowned)
        {
            _downedUnitsInRange.Add(tar);

            if (_reviveTarget != null && _reviveTarget != tar)
                if (Vector3.Distance(transform.position, tar.transform.position) < Vector3.Distance(transform.position, _reviveTarget.transform.position))
                    _reviveTarget = tar;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerDownedScript>(out PlayerDownedScript tar) && tar.IsDowned)
        {
            _downedUnitsInRange.Remove(tar);

            if (_downedUnitsInRange.Count == 0)
                _reviveTarget = null;
            else
            {
                var closestTarget = _downedUnitsInRange[0];

                foreach (var target in _downedUnitsInRange)
                    if (Vector3.Distance(transform.position, target.transform.position) < Vector3.Distance(transform.position, closestTarget.transform.position))
                        closestTarget = target;

                _reviveTarget = closestTarget;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            _isReviving = true;
            _reviveTimer += 1f;
            StartCoroutine(ReviveTargetCoro());
        }
        if (Input.GetKey(KeyCode.R))
        {
            _reviveTimer += Time.deltaTime;
        }
        if (Input.GetKeyUp(KeyCode.R))
        {
            StopAllCoroutines();
            _isReviving = false;
        }
    }
    public IEnumerator ReviveTargetCoro()
    {
        while (_reviveTimer > 0f && !_isReviving)
        {
            _reviveTimer -= 0.5f * Time.deltaTime;

            if (_reviveTimer >= _reviveChannelTime)
            {
                OnCoroComplete(false);
                yield break;
            }

            yield return null;
        }

        OnCoroComplete(true);
    }
    public void OnCoroComplete(bool isSuccess)
    {
        //if (isSuccess)

    }
}
