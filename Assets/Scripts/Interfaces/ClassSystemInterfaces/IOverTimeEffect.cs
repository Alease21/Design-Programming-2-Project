using AbilitySystem;
using System;

public interface IOverTimeEffect
{
    public abstract void StartTimedEffect(AbilityData abilityData, Action onFinished);
}
