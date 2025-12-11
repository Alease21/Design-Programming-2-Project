using AbilitySystem;
using NavMeshPlus.Components;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using WFC;

public enum EnemyStates
{
    None,
    Idle,
    Roam,
    Chase,
    Attack
}
public class EnemyFSM : MonoBehaviourPunCallbacks
{
    [SerializeField] private EnemyStates _enemyState = EnemyStates.None;

    [SerializeField] private float _idleDuration;
    [SerializeField] private float _roamRadius;
    [SerializeField] private AbilityDefinition _attack;
    [SerializeField] private float _tempAttackRange;
    [SerializeField] private float _sightRange;
    [SerializeField] private GameObject _curTargetGO;
    [SerializeField] private Dictionary<GameObject, float> playersDict = new();
    [SerializeField] private float _pathingAllowance;
    [SerializeField] private bool _attackOnCooldown = false;
    //[SerializeField] private int _enemyID;

    private NavMeshAgent _navMeshAgent;
    private UnitScript _unitScript;
    private Coroutine _idleCoro = null;

    public UnitScript GetCurTarget => _curTargetGO.GetComponent<UnitScript>();
    public int GetEnemyID => photonView.ViewID;

    private void Awake()
    {
        GameManager.instance.enemies.Add(this);
        photonView.ViewID = GameManager.instance.enemies.IndexOf(this) + 10;

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _unitScript = GetComponent<UnitScript>();
        _attack = _unitScript.GetCharacterClass.GetBasicAbility;
    }
    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (!DungeonCreator.instance.IsNavMeshBaked) return;

        CheckForPlayer();
        //photonView.RPC(nameof(CheckForPlayer), RpcTarget.Others, GetEnemyID);

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
                photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Idle);
                //SwapState(EnemyStates.Idle);
            return;
        }
        int playerID = closest.Key.GetComponent<PlayerMultiplayerIdScript>().id;

        if (_curTargetGO == null || (_curTargetGO != null && playerID != _curTargetGO.GetComponent<PlayerMultiplayerIdScript>().id))
            photonView.RPC(nameof(SetCurrentTarget), RpcTarget.All, GetEnemyID, playerID);
            //_curTargetGO = closest.Key;

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, (Vector2)(_curTargetGO.transform.position - transform.position).normalized, _sightRange, LayerMask.GetMask("Player", "Default"));
        if (hit == false || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            if (_enemyState != EnemyStates.Idle && _enemyState != EnemyStates.Roam)
                photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Idle);
                //SwapState(EnemyStates.Idle);
            return;
        }

        if (closest.Value <= _tempAttackRange)
        {
            if (_enemyState != EnemyStates.Attack)
                photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Attack);
                //SwapState(EnemyStates.Attack);
        }
        else
            if (_enemyState != EnemyStates.Chase)
            photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Chase);
            //SwapState(EnemyStates.Chase);
    }
    [PunRPC]
    public void SetCurrentTarget(int id, int targetID)
    {
        if (id != GetEnemyID) return;

        foreach (var p in GameManager.instance.players)
            if (p.id == targetID)
            {
                _curTargetGO = p.gameObject;
                break;
            }
    }

    [PunRPC]
    private void SwapState(int id, EnemyStates newState)
    {
        if (id != GetEnemyID) return;

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
            photonView.RPC(nameof(StartIdleCoroutine), RpcTarget.All, GetEnemyID);
            //_idleCoro = StartCoroutine(IdleWaitCoro());
    }
    [PunRPC]
    private void StartIdleCoroutine(int id)
    {
        if (id != GetEnemyID) return;

        _idleCoro = StartCoroutine(IdleWaitCoro());
    }
    private IEnumerator IdleWaitCoro()
    {
        _enemyState = EnemyStates.Idle;
        yield return new WaitForSeconds(_idleDuration);
        //photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Roam);
        SwapState(GetEnemyID, EnemyStates.Roam);
        _idleCoro = null;
    }

    private void RoamActions()
    {
        if (_navMeshAgent.hasPath) return;

        if (_navMeshAgent.velocity.magnitude > 0f && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance + _pathingAllowance)
        {
            photonView.RPC(nameof(SwapToIdleFromRoam), RpcTarget.All, GetEnemyID);
            //_navMeshAgent.ResetPath();
            //SwapState(EnemyStates.Idle);
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
        } while (hit == true && !onNavmesh);

        photonView.RPC(nameof(SetRoamTargetDestination), RpcTarget.All, GetEnemyID, roamPos);
        //_navMeshAgent.SetDestination(roamPos + (Vector2)transform.position);
    }
    [PunRPC]
    private void SwapToIdleFromRoam(int id)
    {
        if (id != GetEnemyID) return;

        _navMeshAgent.ResetPath();
        //photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Idle);
        SwapState(GetEnemyID, EnemyStates.Idle);
    }

    [PunRPC]
    private void SetRoamTargetDestination(int id, Vector2 roamPos)
    {
        if (id != GetEnemyID) return;

        _navMeshAgent.SetDestination(roamPos + (Vector2)transform.position);
    }

    private void ChaseActions()
    {
        if (_curTargetGO != null)
            photonView.RPC(nameof(SetChaseTargetDestination), RpcTarget.All, GetEnemyID);
            //_navMeshAgent.SetDestination(_curTargetGO.transform.position);
        
        if (_navMeshAgent.hasPath)
            if(_navMeshAgent.remainingDistance <= _tempAttackRange + _pathingAllowance)
            {
                photonView.RPC(nameof(SwapToAttackFromChase), RpcTarget.All, GetEnemyID);
                //_navMeshAgent.ResetPath();
                //SwapState(EnemyStates.Attack);
                return;
            }
    }
    [PunRPC]
    private void SetChaseTargetDestination(int id)
    {
        if (id != GetEnemyID) return;

        _navMeshAgent.SetDestination(_curTargetGO.transform.position);
    }
    [PunRPC]
    private void SwapToAttackFromChase(int id)
    {
        if (id != GetEnemyID) return;

        _navMeshAgent.ResetPath();
        //photonView.RPC(nameof(SwapState), RpcTarget.All, GetEnemyID, EnemyStates.Attack);
        SwapState(GetEnemyID, EnemyStates.Attack);
    }

    private void AttackActions()
    {
        // use attack action
        if (!_attackOnCooldown)
        {
            photonView.RPC(nameof(StartAttackCoroAndUseAbility), RpcTarget.All, GetEnemyID);

            //StartCoroutine(AttackCooldownCoro());
            //_attack.UseAbility(_unitScript);
        }
    }
    [PunRPC]
    private void StartAttackCoroAndUseAbility(int id)
    {
        if (id != GetEnemyID) return;

        StartCoroutine(AttackCooldownCoro());
        _attack.UseAbility(_unitScript);
    }

    private IEnumerator AttackCooldownCoro()
    {
        _attackOnCooldown = true;
        yield return new WaitForSeconds(_attack.GetRootNode.AbilityCD);
        _attackOnCooldown = false;
    }
}
