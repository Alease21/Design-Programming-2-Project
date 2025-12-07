using UnityEngine;

namespace AbilitySystem
{
    public interface ICanAffectOthers
    {
        public EffectType GetEffectType { get; }
        public LayerMask GetAffectedLayers { get; }
        public float GetTargetingRange { get; }
    }
}