using AbilitySystem;
using System;

public enum AbilityTypes
{
    Physical = 2,
    Magic = 4,
    Fire = 8,
    Poison = 16,
    Healing = 32,
}

public interface IAbilityEffect
{
    public abstract void StartEffect(AbilityData abilityData, Action onFinished);
}
