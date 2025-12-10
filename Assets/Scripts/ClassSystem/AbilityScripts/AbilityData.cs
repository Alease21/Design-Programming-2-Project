using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilitySystem
{
    public class AbilityData
    {
        private IEnumerable<GameObject> _targets;
        private UnitScript _user;
        private AbilityTypes _abilityType;
        private string _abilityAnimName;
        private string _abilityAnimName2;

        public IEnumerable<GameObject> Targets { get { return _targets; } set { _targets = value; } }
        public UnitScript GetUser => _user;
        public AbilityTypes GetAbilityType => _abilityType;
        public string GetAbilityAnimName => _abilityAnimName;
        public string GetAbilityAnimName2 => _abilityAnimName2;

        public AbilityData(UnitScript user, AbilityTypes abilityType)
        {
            _user = user;
            _abilityType = abilityType;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _user?.StartCoroutine(coroutine);
        }

        public void DetermineSpellAnimName(TargetingStrategy tarStrat)
        {
            string animNamePrefix = "";
            string animNameSuffix = "";

            if (GetAbilityType == AbilityTypes.Physical)
            {
                _abilityAnimName = "Arrow";
                _abilityAnimName2 = "Arrow";
                return;
            }
            else if (GetAbilityType == AbilityTypes.Magic)
                animNamePrefix = "Magic";
            else if (GetAbilityType == AbilityTypes.Fire)
                animNamePrefix = "Fire";
            else if (GetAbilityType == AbilityTypes.Poison)
                animNamePrefix = "Poison";
            else if (GetAbilityType == AbilityTypes.Healing)
                animNamePrefix = "Healing";

            if (tarStrat is SelfTarget)
                animNameSuffix = "Aura";
            else if (tarStrat is RadiusTargeting)
                animNameSuffix = "Explosion";
            else if (tarStrat is ProjectileTarget)
                animNameSuffix = "Projectile";

            _abilityAnimName = animNamePrefix + animNameSuffix;
            _abilityAnimName2 = animNamePrefix + "Aura";
        }
    }
}