using UnityEngine;
using XNode;

namespace AbilitySystem
{
	[CreateAssetMenu(fileName = "Ability", menuName = "Ability System/Definition")]
	public class AbilityDefinition : NodeGraph
	{
		private AbilityRootNode _rootNode;

		private AbilityRootNode RootNode
		{
			get
			{
				if(_rootNode == null)
				{
					foreach(AbilityNodeBase node in nodes)// nodes is built in collection of nodes
					{
						if (node is AbilityRootNode)
						{
							_rootNode = node as AbilityRootNode;
						}
					}
				}
				return _rootNode;
			}
		}
		/*
		public void UseAility(AbilityController user)
		{
			RootNode?.UseAbility(user);
		}
		*/
	}
}