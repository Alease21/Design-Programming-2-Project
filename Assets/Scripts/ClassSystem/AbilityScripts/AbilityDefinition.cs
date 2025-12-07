using UnityEngine;
using XNode;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "NewAbility", menuName = "Ability System/New Ability")]
	public class AbilityDefinition : NodeGraph
	{
		private AbilityRootNode _rootNode;

        public AbilityRootNode GetRootNode{ get{
				if(_rootNode == null)
				{
					foreach(AbilityNodeBase node in nodes)
					{
						if (node is AbilityRootNode)
							_rootNode = node as AbilityRootNode;
					}
				}
				return _rootNode;}}

		[SerializeField] public string spellName;
        [TextArea(0,5)]
		[SerializeField] public string spellDescription;
		public void UseAbility(PlayerAbilityController user)
		{
            GetRootNode?.UseAbility(user);
		}
	}
}