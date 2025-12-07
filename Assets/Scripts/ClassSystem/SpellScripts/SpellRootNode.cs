using UnityEngine;
using XNode;

namespace MagicSystem
{
    [CreateNodeMenu("Spell RootNode")]
    public class SpellRootNode : SpellNodeBase
    {
		[Output(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public byte targeting;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public int harmfulEffects;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public bool helpfulEffects;

        [SerializeField] protected SpellElements _spellElement;
        [SerializeField] protected int _manaCost;

        private TargetingStrategy _targetingStrategy;

        public SpellElements GetSpellElement => _spellElement;
        public int GetManaCost => _manaCost;

        public void UseAbility(SpellController user)
		{
            // Check if spell can be cast, return if not, else cast spell
            if (!user.GetComponent<UnitScript>().ChangeMana(_manaCost, false)) return;

            if (_targetingStrategy == null)
                _targetingStrategy = GetPort("targeting").Connection.node as TargetingStrategy;

            SpellData spellData = new SpellData(user, _spellElement);
            spellData.DetermineSpellAnimName(_targetingStrategy);
			_targetingStrategy?.StartTargeting(spellData, () =>
			{
				InitAbility(spellData);
			});
        }

        private void InitAbility(SpellData spellData)
		{
			foreach (NodePort port in Outputs)
			{
				if (port.Connection == null || port.Connection.node == null || port.Connection.node is ISpellEffect == false)
					continue;

                ISpellEffect curEffect = port.Connection.node as ISpellEffect;
				curEffect.StartEffect(spellData, OnEffectFinished);
			}
		}

		private void OnEffectFinished()
		{
            // 
		}

        public override object GetValue(NodePort port)
        {
            if (port.fieldName == "targeting")
                return GetInputValue<bool>("targeting");
            else if (port.IsDynamic)
            {
                if (port.GetConnections().Count == 0) return null;

                if (port.fieldName.Contains("harmfulEffects"))
                    return GetInputValue<byte>("harmfulEffects");
                else if (port.fieldName.Contains("helpfulEffects"))
                    return GetInputValue<byte>("helpfulEffects");
            }
            throw new System.Exception($"{this.GetType()}.GetValue() Override issue");
        }
    }
}