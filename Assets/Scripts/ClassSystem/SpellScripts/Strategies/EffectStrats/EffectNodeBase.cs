using UnityEngine;

namespace MagicSystem
{
    public abstract class EffectNodeBase : SpellNodeBase
    {
        [SerializeField] protected string _effectName;
        [SerializeField] protected UnitStats _affectedStats;
        public bool isOverTime;
        [SerializeField] protected float _duration;
        [SerializeField] protected int _numberOfTicks;
        [SerializeField]
        protected int _staminaValue,
                      _strengthValue,
                      _dexterityValue,
                      _intelligenceValue,
                      _healthValue,
                      _manaValue;

        public float GetDuration { 
            get 
            {
                if (isOverTime)
                    return _duration;
                else
                    return 0f;
            }}
    }
}