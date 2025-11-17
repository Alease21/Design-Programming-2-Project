using MagicSystem;
using System;
using static DamageTypeEnum;

public enum SpellElements
{
    Ice = 4,
    Fire = 8,
    Holy = 16,
    Unholy = 32
}

public interface ISpellEffect
{
    public abstract void StartEffect(SpellData abilityData, Action onFinished);
}
