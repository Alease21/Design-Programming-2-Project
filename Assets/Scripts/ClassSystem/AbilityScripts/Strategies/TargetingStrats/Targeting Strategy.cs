using System;
using UnityEngine;

public enum EffectType
{
    None = 0,
    All = -1,
    Harmful = 2,
    Helpful = 4,
    Misc = 8
}

namespace AbilitySystem
{
    public abstract class TargetingStrategy : AbilityNodeBase
    {
        [Input(connectionType = ConnectionType.Override)] public byte input;

        [SerializeField, NodeEnum] protected EffectType _effectType;
        [SerializeField] protected LayerMask _affectedLayers;
        [SerializeField] protected float _range;
        [SerializeField] protected float _projectileSpeed;

        public abstract void StartTargeting(AbilityData abilityData, Action onFinished);
    }
}