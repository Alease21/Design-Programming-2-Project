using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateNodeMenu("Harmful Effects/Debuff")]
    public class DebuffEffect : EffectNodeBase, IAbilityEffect, IAuraEffect
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public int input;

        public string GetEffectName => _effectName;
        public UnitStats GetAffectedStats => _affectedStats;
        public int GetStaminaModifierValue => _staminaValue;
        public int GetStrengthModifierValue => _strengthValue;
        public int GetDexterityModifierValue => _dexterityValue;
        public int GetIntelligenceModifierValue => _intelligenceValue;

        private GameObject _effect;

        public void StartEffect(AbilityData abilityData, Action onFinished)
        {
            foreach (var target in abilityData.Targets)
            {
                UnitScript uss = target.GetComponent<UnitScript>();
                if (uss != null)
                {
                    uss.StartCoroutine(StatEffectOverDuration(_duration, uss));
                    _effect = Instantiate(Resources.Load<GameObject>("AbilityEffects/AuraEffectSprite"), uss.transform.position, Quaternion.identity, uss.transform);
                    _effect.GetComponent<Animator>().Play(abilityData.GetAbilityAnimName2);
                }
            }
        }
        public IEnumerator StatEffectOverDuration(float duration, UnitScript target)
        {
            int[] statVals = new int[4];
            List<string> statTypes = new List<string>(GetAffectedStats.ToString().Split(", "));

            if (statTypes.Contains("Stamina") || statTypes.Contains("-1"))
                statVals[0] = GetStaminaModifierValue;
            if (statTypes.Contains("Strength") || statTypes.Contains("-1"))
                statVals[1] = GetStrengthModifierValue;
            if (statTypes.Contains("Dexterity") || statTypes.Contains("-1"))
                statVals[2] = GetDexterityModifierValue;
            if (statTypes.Contains("Intelligence") || statTypes.Contains("-1"))
                statVals[3] = GetIntelligenceModifierValue;

            target.UpdateStats(statVals, false); //apply debuff
            yield return new WaitForSeconds(duration);
            Destroy(_effect);
            target.UpdateStats(statVals, true); //undo debuff
        }
    }
}