using UnityEngine;
using XNode;

namespace AbilitySystem
{
    [CreateNodeMenu("Ability RootNode")]
    public class AbilityRootNode : AbilityNodeBase
    {
		[Output(connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public byte targeting;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public int harmfulEffects;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public bool helpfulEffects;
        [Output(dynamicPortList = true, connectionType = ConnectionType.Override, typeConstraint = TypeConstraint.Strict)] public float miscEffects;

        [SerializeField] protected AbilityTypes _abilityType;
        [SerializeField] private float _abilityCD;

        private TargetingStrategy _targetingStrategy;

        public AbilityTypes GetAbilityType => _abilityType;
        public float AbilityCD => _abilityCD;

        public void UseAbility(UnitScript user)
		{
            if (_targetingStrategy == null)
                _targetingStrategy = GetPort("targeting").Connection.node as TargetingStrategy;

            AbilityData AbilityData = new AbilityData(user, _abilityType);
            AbilityData.DetermineAbilityAnimName(_targetingStrategy);
			_targetingStrategy?.StartTargeting(AbilityData, () =>
			{
				InitAbility(AbilityData);
			});
        }

        private void InitAbility(AbilityData AbilityData)
		{
			foreach (NodePort port in Outputs)
			{
				if (port.Connection == null || port.Connection.node == null || port.Connection.node is IAbilityEffect == false)
					continue;

                IAbilityEffect curEffect = port.Connection.node as IAbilityEffect;
				curEffect.StartEffect(AbilityData, OnEffectFinished);
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