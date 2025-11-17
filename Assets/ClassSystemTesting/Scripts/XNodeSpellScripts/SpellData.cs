using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicSystem
{
    public class SpellData
    {
        private IEnumerable<GameObject> _targets;
        private SpellController _user;
        private SpellElements _spellElement;
        private string _spellAnimName;

        public IEnumerable<GameObject> Targets { get { return _targets; } set { _targets = value; } }
        public SpellController GetUser => _user;
        public SpellElements GetSpellElement => _spellElement;
        public string GetSpellAnimName => _spellAnimName;

        public SpellData(SpellController user, SpellElements spellElement)
        {
            _user = user;
            _spellElement = spellElement;
        }

        public void StartCoroutine(IEnumerator coroutine)
        {
            _user?.StartCoroutine(coroutine);
        }

        public void DetermineSpellAnimName(TargetingStrategy tarStrat)
        {
            string animNamePrefix = "";
            string animNameSuffix = "";
            if (GetSpellElement == SpellElements.Fire)
                animNamePrefix = "Fire";
            else if (GetSpellElement == SpellElements.Ice)
                animNamePrefix = "Ice";
            else if (GetSpellElement == SpellElements.Holy)
                animNamePrefix = "Holy";
            else if (GetSpellElement == SpellElements.Unholy)
                animNamePrefix = "Unholy";

            if (tarStrat is SelfTarget)
                animNameSuffix = "Aura";
            else if (tarStrat is RadiusTargeting)
                animNameSuffix = "Explosion";
            else if (tarStrat is ProjectileTarget)
                animNameSuffix = "Projectile";

            _spellAnimName = animNamePrefix + animNameSuffix;
        }
    }
}