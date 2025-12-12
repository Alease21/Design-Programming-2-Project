using AbilitySystem;
using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(UnitScript))]
public class PlayerAbilityController : MonoBehaviourPunCallbacks
{
    private UnitScript _unitScript;
    [SerializeField] private AbilityDefinition _basicAbility;
    [SerializeField] private AbilityDefinition _ultimateAbility;

    private bool _basicOnCooldown = false;
    private bool _ultimateOnCooldown = false;
    private Vector3 _trueMouseDir;

    private PlayerMultiplayerIdScript _multiplayerIdScript;

    public Vector3 GetTrueMouseDir => _trueMouseDir;

    private void Awake()
    {
        if (!TryGetComponent<UnitScript>(out _unitScript)) return;
        _unitScript.PlayerClassLoaded += GrabAbilities;

        _multiplayerIdScript = GetComponent<PlayerMultiplayerIdScript>();
    }
    private void OnDestroy()
    {
        _unitScript.PlayerClassLoaded -= GrabAbilities;
    }
    public void GrabAbilities()
    {
        _basicAbility = _unitScript.GetCharacterClass.GetBasicAbility;
        _ultimateAbility = _unitScript.GetCharacterClass.GetUltimateAbility;
        Debug.Log($"unit: {gameObject.name}, class {_unitScript.GetCharacterClass.name}, basic: {_basicAbility?.name}, ult: {_ultimateAbility?.name}");
    }
    private void Update()
    {
        if (!photonView.IsMine) return;

        if (Input.GetMouseButtonDown(0))
            if (!_basicOnCooldown && _basicAbility != null)
            {
                _trueMouseDir = GetComponent<PlayerMovement>().GetMouseDir;
                photonView.RPC(nameof(StartBasicAbility), RpcTarget.All, _multiplayerIdScript.id, _trueMouseDir);
            }

        if (Input.GetKeyDown(KeyCode.Q))
            if (!_ultimateOnCooldown && _ultimateAbility != null)
            {
                _trueMouseDir = GetComponent<PlayerMovement>().GetMouseDir;
                photonView.RPC(nameof(StartUltimateAbility), RpcTarget.All, _multiplayerIdScript.id, _trueMouseDir);
            }
    }
    [PunRPC]
    public void StartBasicAbility(int id, Vector3 mouseDir)
    {
        if (id == _multiplayerIdScript.id)
        {
            _trueMouseDir = mouseDir;
            StartCoroutine(BasicAbilityCoolDownCoro());
        }
    }
    [PunRPC]
    public void StartUltimateAbility(int id, Vector3 mouseDir)
    {
        if (id == _multiplayerIdScript.id)
        {
            _trueMouseDir = mouseDir;
            StartCoroutine(UltimateAbilityCoolDownCoro());
        }
    }
    public IEnumerator BasicAbilityCoolDownCoro()
    {
        _basicAbility?.UseAbility(_unitScript);
        _basicOnCooldown = true;
        float abilityCD = _basicAbility.GetRootNode.AbilityCD;

        for (float timer = 0f; timer < abilityCD; timer += Time.deltaTime)
        {
            //update a ui thing?
            yield return null;
        }

        _basicOnCooldown = false;
    }
    public IEnumerator UltimateAbilityCoolDownCoro()
    {
        _ultimateAbility?.UseAbility(_unitScript);
        _ultimateOnCooldown = true;
        float abilityCD = _ultimateAbility.GetRootNode.AbilityCD;

        for (float timer = 0f; timer < abilityCD; timer += Time.deltaTime)
        {
            //update a ui thing?
            yield return null;
        }

        _ultimateOnCooldown = false;
    }
}
