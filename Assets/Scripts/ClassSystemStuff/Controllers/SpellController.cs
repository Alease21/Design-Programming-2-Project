using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MagicSystem
{
    public class SpellController : MonoBehaviour
    {
        [SerializeField] protected SpellDefinition _spell1;
        [SerializeField] protected SpellDefinition _spell2;
        [SerializeField] protected SpellDefinition _spell3;
        [SerializeField] protected SpellDefinition _spell4;

        private Dictionary<SpellDefinition, bool> _spells = new();

        private UnitScript _unit;

        public Dictionary<SpellDefinition, bool> GetSpells => _spells;
        public UnitScript GetUnit => _unit;

        private void Start()
        {
            SetSpells();
        }

        private void Update()
        {
            if (_unit.GetUnitType != UnitTypes.Player && _unit.GetUnitType != UnitTypes.Friendly) return;

            if (Input.GetKeyDown(KeyCode.Alpha1))
                CheckValidAndUseSpell(_spell1);
            if (Input.GetKeyDown(KeyCode.Alpha2))
                CheckValidAndUseSpell(_spell2);
            if (Input.GetKeyDown(KeyCode.Alpha3))
                CheckValidAndUseSpell(_spell3);
            if (Input.GetKeyDown(KeyCode.Alpha4))
                CheckValidAndUseSpell(_spell4);
        }

        private void CheckValidAndUseSpell(SpellDefinition spell)
        {
            if (_unit == null) _unit = GetComponent<UnitScript>();

            if (_spells[spell] == true)
                spell.UseAility(this);
            else
                Debug.LogError("Attempted Invalid Spell Use");
        }
        public bool[] SetSpells()
        {
            _unit = GetComponent<UnitScript>();
            if (_unit == null) return null;

            if (_unit.GetCharacterClass == null) return null;

            _unit.UpdateSpellInfo();

            Dictionary<SpellDefinition, bool> spells = new();
            SpellDefinition[] equippedSpells = { _spell1, _spell2, _spell3, _spell4 };
            List<bool> tempBoolList = new();

            for (int i = 0; i < equippedSpells.Length; i++)
            {
                SpellDefinition spell = equippedSpells[i];
                if (spell == null) continue;

                bool isValid = _unit.GetAllowedMagic.Contains(equippedSpells[i].GetRootNode.GetSpellElement.ToString()) ||
                    _unit.GetAllowedMagic.Contains("All");

                tempBoolList.Add(isValid);
                spells.Add(equippedSpells[i], isValid);
            }

            _spells = spells;
            return tempBoolList.ToArray();
        }
    }
}