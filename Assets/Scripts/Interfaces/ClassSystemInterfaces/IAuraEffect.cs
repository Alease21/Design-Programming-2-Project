namespace MagicSystem
{
    public interface IAuraEffect
    {
        public string GetEffectName { get; }
        public UnitStats GetAffectedStats { get; }
        public int GetStaminaModifierValue { get; }
        public int GetStrengthModifierValue { get; }
        public int GetDexterityModifierValue { get; }
        public int GetIntelligenceModifierValue { get; } 
    }
}