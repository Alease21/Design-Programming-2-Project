using System;
using System.Collections;
using UnityEngine;
using static DamageTypeEnum;
using static UnityEngine.GraphicsBuffer;

namespace AbilitySystem
{
    [CreateNodeMenu("Helpful Effects/Heal")]
    public class HealEffect : EffectNodeBase, IAbilityEffect
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public bool input;

        public void StartEffect(AbilityData AbilityData, Action onFinished)
        {
            foreach (var target in AbilityData.Targets)
            {
                UnitScript uss = target.GetComponent<UnitScript>();
                if (uss != null)
                {
                    if (!isOverTime)
                    {
                        int exp = uss.ChangeHealth(_healthValue, true);
                        if (exp != 0)
                            AbilityData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);
                    }
                    else
                        uss.StartCoroutine(HealOverTimeCoro(_healthValue, _duration, uss, AbilityData));
                }
            }
        }
        private IEnumerator HealOverTimeCoro(int amount, float duration, UnitScript target, AbilityData AbilityData)
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("AuraEffectSprite"), target.transform.position, Quaternion.identity, target.transform);
            effect.GetComponent<Animator>().Play(AbilityData.GetAbilityAnimName);

            int newAmount = amount;
            float tickTime = duration / _numberOfTicks;
            int tickVal = amount / _numberOfTicks;
            do
            {
                int exp = target.ChangeHealth(tickVal, true);
                if (exp != 0)
                    AbilityData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                newAmount -= tickVal;
                yield return new WaitForSeconds(tickTime);
            } while (newAmount > 0 && target != null);

            Destroy(effect);
        }
    }
}