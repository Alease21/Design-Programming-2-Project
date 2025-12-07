using System;
using System.Collections;
using UnityEngine;
using static DamageTypeEnum;
using static UnityEngine.GraphicsBuffer;

namespace MagicSystem
{
    [CreateNodeMenu("Helpful Effects/Heal")]
    public class HealEffect : EffectNodeBase, ISpellEffect
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public bool input;

        public void StartEffect(SpellData spellData, Action onFinished)
        {
            foreach (var target in spellData.Targets)
            {
                UnitScript uss = target.GetComponent<UnitScript>();
                if (uss != null)
                {
                    if (!isOverTime)
                    {
                        int exp = uss.ChangeHealth(_healthValue, true);
                        if (exp != 0)
                            spellData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);
                    }
                    else
                        uss.StartCoroutine(HealOverTimeCoro(_healthValue, _duration, uss, spellData));
                }
            }
        }
        private IEnumerator HealOverTimeCoro(int amount, float duration, UnitScript target, SpellData spellData)
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("AuraEffectSprite"), target.transform.position, Quaternion.identity, target.transform);
            effect.GetComponent<Animator>().Play(spellData.GetSpellAnimName);


            int newAmount = amount;
            float tickTime = duration / _numberOfTicks;
            int tickVal = amount / _numberOfTicks;
            do
            {
                int exp = target.ChangeHealth(tickVal, true);
                if (exp != 0)
                    spellData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                newAmount -= tickVal;
                yield return new WaitForSeconds(tickTime);
            } while (newAmount > 0 && target != null);

            Destroy(effect);
        }
    }
}