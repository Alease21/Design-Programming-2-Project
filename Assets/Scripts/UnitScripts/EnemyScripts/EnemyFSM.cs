using NavMeshPlus.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates
{
    None,
    Idle,
    Roam,
    Chase,
    Attack
}
public class EnemyFSM : MonoBehaviour
{
    [SerializeField] private EnemyStates _enemyState = EnemyStates.None;

    [SerializeField] private float _idleDuration;
    [SerializeField] private float _roamRadius;
    //[SerializeField] private EnemyAttackSO _attackSO;
    [SerializeField] private float tempattackCD;
    [SerializeField] private float _tempAttackRange;
    [SerializeField] private float _sightRange;
    [SerializeField] private GameObject _curTargetGO;
    [SerializeField] private Dictionary<GameObject, float> playersDict = new();
    [SerializeField] private float _pathingAllowance;
    [SerializeField] private bool _attackOnCooldown = false;

    //private EnemyScript _enemyScript;
    private NavMeshAgent _navMeshAgent;
    private Coroutine _idleCoro = null;

    public UnitScript GetCurTarget => _curTargetGO.GetComponent<UnitScript>();

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        CheckForPlayer();

        switch (_enemyState)
        {
            case EnemyStates.Roam:
                RoamActions();
                break;
            case EnemyStates.Chase:
                ChaseActions();
                break;
            case EnemyStates.Attack:
                AttackActions();
                break;
            default:
                IdleActions();
                break;
        }
    }
    private void CheckForPlayer()
    {
        if (_curTargetGO == null)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                if (!playersDict.ContainsKey(player))
                    playersDict.Add(player, Vector2.Distance(transform.position, player.transform.position));

            if (playersDict.Count == 0) return;
        }

        for (int i = 0; i < playersDict.Count; i++)
            playersDict[playersDict.ElementAt(i).Key] = Vector2.Distance(transform.position, playersDict.ElementAt(i).Key.transform.position);
        var sortedPlayers = playersDict.OrderBy(x => x.Value);
        var closest = sortedPlayers.First();

        if (closest.Value > _sightRange)
        {
            _curTargetGO = null;
            if (_enemyState != EnemyStates.Idle && _enemyState != EnemyStates.Roam)
                SwapState(EnemyStates.Idle);
            return;
        }

        _curTargetGO = closest.Key;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)(_curTargetGO.transform.position - transform.position).normalized, _sightRange, LayerMask.GetMask("Player", "Default"));
        if (hit == false || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (_enemyState != EnemyStates.Idle && _enemyState != EnemyStates.Roam)
                SwapState(EnemyStates.Idle);
            return;
        }

        if (closest.Value <= _tempAttackRange)
        {
            if (_enemyState != EnemyStates.Attack)
                SwapState(EnemyStates.Attack);
        }
        else
            if (_enemyState != EnemyStates.Chase)
                SwapState(EnemyStates.Chase);
    }
    private void SwapState(EnemyStates newState)
    {
        _enemyState = newState;

        if (_idleCoro != null)
        {
            StopCoroutine(_idleCoro);
            _idleCoro = null;
        }
    }
    private void IdleActions()
    {
        if (_idleCoro == null)
            _idleCoro = StartCoroutine(IdleWaitCoro());
    }
    private IEnumerator IdleWaitCoro()
    {
        _enemyState = EnemyStates.Idle;
        yield return new WaitForSeconds(_idleDuration);
        SwapState(EnemyStates.Roam);
        _idleCoro = null;
    }
    private void RoamActions()
    {
        if (_navMeshAgent.hasPath) return;

        if (_navMeshAgent.velocity.magnitude > 0f && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + _pathingAllowance)
        {
            _navMeshAgent.ResetPath();
            SwapState(EnemyStates.Idle);
            return;
        }

        Vector2 roamPos = Vector2.zero;
        RaycastHit2D hit;
        bool onNavmesh;
        do
        {
            roamPos = new Vector2(UnityEngine.Random.Range(-_roamRadius, _roamRadius), UnityEngine.Random.Range(-_roamRadius, _roamRadius));
            hit = Physics2D.Raycast(transform.position, roamPos.normalized, roamPos.magnitude, LayerMask.GetMask("Default"));
            onNavmesh = NavMesh.SamplePosition(roamPos + (Vector2)transform.position, out NavMeshHit navHit, roamPos.magnitude, NavMesh.GetAreaFromName("Walkable"));
        } while (hit == true && !onNavmesh && _navMeshAgent.SetDestination(roamPos + (Vector2)transform.position));
    }
    private void ChaseActions()
    {
        if (_curTargetGO != null)
            _navMeshAgent.SetDestination(_curTargetGO.transform.position);
        
        if (_navMeshAgent.hasPath)
            if(_navMeshAgent.remainingDistance <= /*_attackSO.range*/ _tempAttackRange + _pathingAllowance)
            {
                //Debug.Log($"remaining dist: {_navMeshAgent.remainingDistance}, range: {/*_attackSO.range*/ _tempAttackRange}, allow {_pathingAllowance}");
                _navMeshAgent.ResetPath();
                SwapState(EnemyStates.Attack);
                return;
            }
    }
    private void AttackActions()
    {
        // use attack action
        if (!_attackOnCooldown)
        {
            StartCoroutine(AttackCooldownCoro());
            Debug.Log("Attack Used");
        }
    }
    private IEnumerator AttackCooldownCoro()
    {
        _attackOnCooldown = true;
        yield return new WaitForSeconds(/*_attackSO.miniCooldown*/ tempattackCD);
        _attackOnCooldown = false;
    }
}
