using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace AbilitySystem
{
    [CreateNodeMenu("Misc Effects/SpawnObjectAtLocation")]
    public class SpawnObjectTargetLocation : EffectNodeBase, IAbilityEffect
    {
        public GameObject objectToSpawn;
        public bool isFriendly;

        [Input(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public float input;

        public void StartEffect(AbilityData abilityData, Action onFinished)
        {
            foreach (var target in abilityData.Targets)
            {
                GameObject obj = Instantiate(objectToSpawn, target.transform.position, Quaternion.identity);
                var script = obj.GetComponent<CloudObjectScript>();
                script.duration = _duration;
                script.isFriendly = isFriendly;
                script.StartCoroutine(script.CloudRoutine());
            }
        }
    }
}