using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using Photon.Pun;

public class PlayerReviveScript : MonoBehaviourPunCallbacks
{
    [SerializeField] private CircleCollider2D _reviveTrigger;
    [SerializeField] private float _reviveRadius = 2f;
    [SerializeField] private List<PlayerDownedScript> _downedUnitsInRange = new();
    [SerializeField] private PlayerDownedScript _reviveTarget;
    [SerializeField] private float _reviveChannelTime = 5f;
    [SerializeField] private bool _isReviving = false;
    [SerializeField] private float _reviveTimer = 0f;

    private void Awake()
    {
        _reviveTrigger = GetComponent<CircleCollider2D>();
        _reviveTrigger.radius = _reviveRadius;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<PlayerDownedScript>(out PlayerDownedScript tar) && tar.IsDowned)
        {
            _downedUnitsInRange.Add(tar);

            if (_reviveTarget != null && _reviveTarget != tar)
            {
                if (Vector3.Distance(transform.position, tar.transform.position) < Vector3.Distance(transform.position, _reviveTarget.transform.position))
                    _reviveTarget = tar;
            }
            else
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
        if (!photonView.IsMine || _reviveTarget == null) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            _isReviving = true;
            _reviveTimer += Time.deltaTime;
            StartCoroutine(ReviveTargetCoro());
        }
        if (Input.GetKey(KeyCode.R))
        {
            _reviveTimer += Time.deltaTime;
            //update ui?

            if (_reviveTimer >= _reviveChannelTime)
            {
                StopAllCoroutines();
                _reviveTarget.photonView.RPC(nameof(PlayerDownedScript.OnPlayerRevived), RpcTarget.All, _reviveTarget.GetComponent<PlayerMultiplayerIdScript>().id);
                _downedUnitsInRange = new();
                _reviveTarget = null;
                _isReviving = false;
                _reviveTimer = 0f;
            }
        }

        if (Input.GetKeyUp(KeyCode.R))
            _isReviving = false;
    }
    public IEnumerator ReviveTargetCoro()
    {
        while (_reviveTimer > 0f)
        {
            if (!_isReviving)
                _reviveTimer -= Time.deltaTime;

            //update ui?
            yield return null;
        }
    }
}
