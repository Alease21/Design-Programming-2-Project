using System;
using UnityEngine;

public enum EffectType
{
    None = 0,
    Both = -1,
    Harmful = 2,
    Helpful = 4,
}

namespace MagicSystem
{
    public abstract class TargetingStrategy : SpellNodeBase
    {
        [Input(connectionType = ConnectionType.Override)] public byte input;

        [SerializeField, NodeEnum] protected EffectType _effectType;
        [SerializeField] protected LayerMask _affectedLayers;
        [SerializeField] protected float _range;
        [SerializeField] protected float _projectileSpeed;


        public abstract void StartTargeting(SpellData spellData, Action onFinished);
    }
}