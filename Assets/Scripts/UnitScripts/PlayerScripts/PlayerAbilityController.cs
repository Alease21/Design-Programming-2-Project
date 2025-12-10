using AbilitySystem;
using System.Collections;
using UnityEditor.Playables;
using UnityEngine;

[RequireComponent(typeof(UnitScript))]
public class PlayerAbilityController : MonoBehaviour
{
    private UnitScript _unitScript;
    private AbilityDefinition _basicAbility;
    private AbilityDefinition _ultimateAbility;

    private bool _basicOnCooldown = false;
    private bool _ultimateOnCooldown = false;

    private void Awake()
    {
        if (!TryGetComponent<UnitScript>(out _unitScript)) return;

        _basicAbility = _unitScript.GetCharacterClass.GetBasicAbility;
        _ultimateAbility = _unitScript.GetCharacterClass.GetUltimateAbility;
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            if (!_basicOnCooldown && _basicAbility != null)
                StartCoroutine(BasicAbilityCoolDownCoro());

        if (Input.GetKeyDown(KeyCode.Q))
            if (!_ultimateOnCooldown && _ultimateAbility != null)
                StartCoroutine(UltimateAbilityCoolDownCoro());
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
