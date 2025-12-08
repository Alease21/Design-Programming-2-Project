using MagicSystem;
using System;

public interface IOverTimeEffect
{
    public abstract void StartTimedEffect(SpellData abilityData, Action onFinished);
}
