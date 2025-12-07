using System;
using System.Collections;
using UnityEngine;
using static DamageTypeEnum;

namespace AbilitySystem
{
    [CreateNodeMenu("Harmful Effects/Damage")]
    public class DamageEffect : EffectNodeBase, IAbilityEffect
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public int input;

        public void StartEffect(AbilityData AbilityData, Action onFinished)
        {
            if (AbilityData.Targets == null) return;
            foreach (var target in AbilityData.Targets)
            {
                UnitScript uss = target.GetComponent<UnitScript>();
                if (uss != null)
                {
                    if (!isOverTime)
                    {
                        var dmgType = GetAllTypesFromFlags((DamageTypes)AbilityData.GetAbilityType);
                        
                        int exp = uss.ChangeHealth(_healthValue, false, dmgType);
                        if (exp != 0)
                            AbilityData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                    }
                    else
                        uss.StartCoroutine(DamageOverTimeCoro(_healthValue, _duration, uss, AbilityData));
                }
            }
        }

        private IEnumerator DamageOverTimeCoro(int amount, float duration, UnitScript target, AbilityData AbilityData)
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("AbilityEffects/AuraEffectSprite"), target.transform.position, Quaternion.identity, target.transform);
            effect.GetComponent<Animator>().Play(AbilityData.GetAbilityAnimName);

            int newAmount = amount;
            float tickTime = duration / _numberOfTicks;
            int tickVal = amount / _numberOfTicks;
            var dmgType = GetAllTypesFromFlags((DamageTypes)AbilityData.GetAbilityType);

            do
            {
                int exp = target.ChangeHealth(tickVal, false, dmgType);
                if (exp != 0)
                    AbilityData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                newAmount -= tickVal;
                yield return new WaitForSeconds(tickTime);
            } while (newAmount > 0 && target != null);

            Destroy(effect);
        }
    }
}