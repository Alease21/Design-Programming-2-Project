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
    private EnemyStates _enemyState = EnemyStates.None;

    [SerializeField] private float _idleDuration;
    [SerializeField] private float _roamRadius;
    [SerializeField] private EnemyAttackSO _attackSO;
    [SerializeField] private GameObject _curTarget;
    [SerializeField] private Dictionary<GameObject, float> playersDict = new();
    [SerializeField] private float _pathingAllowance;
    private bool _attackOnCooldown = false;

    private EnemyScript _enemyScript;
    private NavMeshAgent _navMeshAgent;

    private void Awake()
    {
        _enemyScript = GetComponent<EnemyScript>();
        _navMeshAgent = GetComponent<NavMeshAgent>();

        _idleDuration = _enemyScript.GetEnemyBaseStats.idleDuration;
        _roamRadius = _enemyScript.GetEnemyBaseStats.roamRadius;
        _attackSO = _enemyScript.GetEnemyBaseStats.attackSO;
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
        if (_curTarget == null)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
                if (!playersDict.ContainsKey(player))
                    playersDict.Add(player, Vector2.Distance(transform.position, player.transform.position));

            if (playersDict.Count == 0) return;
        }

        for (int i = 0; i < playersDict.Count; i++)
            playersDict[playersDict.ElementAt(i).Key] = Vector2.Distance(transform.position, playersDict.ElementAt(i).Key.transform.position);
        var sortedPlayers = playersDict.OrderBy(x => x.Value);
        if (sortedPlayers.First().Value <= _attackSO.range)
        {
            _curTarget = sortedPlayers.First().Key;
            SwapState(EnemyStates.Chase);
        }
        else
            _curTarget = null;
    }
    private void SwapState(EnemyStates newState)
    {
        _enemyState = newState;
        StopAllCoroutines();
    }
    private void IdleActions()
    {
        if (_enemyState != EnemyStates.Idle)
            StartCoroutine(IdleWaitCoro());
    }
    private IEnumerator IdleWaitCoro()
    {
        _enemyState = EnemyStates.Idle;
        yield return new WaitForSeconds(_idleDuration);
        SwapState(EnemyStates.Roam);
    }
    private void RoamActions()
    {
        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + _pathingAllowance)
        {
            _navMeshAgent.ResetPath();
            SwapState(EnemyStates.Idle);
            return;
        }

        if (_navMeshAgent.hasPath) return;

        Vector2 roamPos = Vector2.zero;
        do
        {
            roamPos = new Vector2(UnityEngine.Random.Range(0, _roamRadius), UnityEngine.Random.Range(0, _roamRadius));
        } while (!_navMeshAgent.SetDestination(roamPos));
    }
    private void ChaseActions()
    {
        if (_curTarget != null)
            _navMeshAgent.SetDestination(_curTarget.transform.position);

        if (_navMeshAgent.hasPath)
            if(_navMeshAgent.remainingDistance <= _attackSO.range + _pathingAllowance)
            {
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
        yield return new WaitForSeconds(_attackSO.miniCooldown);
        _attackOnCooldown = false;
    }
}
