using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicSystem
{
    [CreateNodeMenu("Targeting/Self")]
    public class SelfTarget : TargetingStrategy
    {
        public override void StartTargeting(SpellData spellData, Action onFinished)
        {
            spellData.Targets = TargetSelf(spellData);
            onFinished();
        }

        private IEnumerable<GameObject>TargetSelf(SpellData abilityData)
        {
            yield return abilityData.GetUser.gameObject;
        }
    }
}