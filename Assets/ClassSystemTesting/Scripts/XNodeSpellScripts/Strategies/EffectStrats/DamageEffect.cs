using System;
using System.Collections;
using UnityEngine;
using static DamageTypeEnum;

namespace MagicSystem
{
    [CreateNodeMenu("Harmful Effects/Damage")]
    public class DamageEffect : EffectNodeBase, ISpellEffect
    {
        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public int input;

        public void StartEffect(SpellData spellData, Action onFinished)
        {
            if (spellData.Targets == null) return;
            foreach (var target in spellData.Targets)
            {
                UnitScript uss = target.GetComponent<UnitScript>();
                if (uss != null)
                {
                    if (!isOverTime)
                    {
                        var dmgType = GetAllTypesFromFlags((DamageTypes)spellData.GetSpellElement);
                        
                        int exp = uss.ChangeHealth(_healthValue, false, dmgType);
                        if (exp != 0)
                            spellData.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                    }
                    else
                        uss.StartCoroutine(DamageOverTimeCoro(_healthValue, _duration, uss, spellData));
                }
            }
        }

        private IEnumerator DamageOverTimeCoro(int amount, float duration, UnitScript target, SpellData spelldata)
        {
            GameObject effect = Instantiate(Resources.Load<GameObject>("AuraEffectSprite"), target.transform.position, Quaternion.identity, target.transform);
            effect.GetComponent<Animator>().Play(spelldata.GetSpellAnimName);

            int newAmount = amount;
            float tickTime = duration / _numberOfTicks;
            int tickVal = amount / _numberOfTicks;
            var dmgType = GetAllTypesFromFlags((DamageTypes)spelldata.GetSpellElement);

            do
            {
                int exp = target.ChangeHealth(tickVal, false, dmgType);
                if (exp != 0)
                    spelldata.GetUser.GetComponent<UnitScript>().ChangeExp(exp, exp > 0 ? true : false);

                newAmount -= tickVal;
                yield return new WaitForSeconds(tickTime);
            } while (newAmount > 0 && target != null);

            Destroy(effect);
        }
    }
}