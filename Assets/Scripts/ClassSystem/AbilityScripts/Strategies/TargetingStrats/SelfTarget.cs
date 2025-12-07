using System;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    [CreateNodeMenu("Targeting/Self")]
    public class SelfTarget : TargetingStrategy
    {
        public override void StartTargeting(AbilityData abilityData, Action onFinished)
        {
            abilityData.Targets = TargetSelf(abilityData);
            onFinished();
        }

        private IEnumerable<GameObject>TargetSelf(AbilityData abilityData)
        {
            yield return abilityData.GetUser.gameObject;
        }
    }
}