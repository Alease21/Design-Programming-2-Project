using MagicSystem;
using System;
using static DamageTypeEnum;

public enum SpellElements
{
    Magic = 4,
    Fire = 8,
    Poison = 16,
    Healing = 32,
}

public interface ISpellEffect
{
    public abstract void StartEffect(SpellData abilityData, Action onFinished);
}
