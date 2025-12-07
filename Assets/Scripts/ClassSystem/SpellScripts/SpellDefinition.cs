using UnityEngine;
using XNode;

namespace MagicSystem
{
	[CreateAssetMenu(fileName = "NewSpell", menuName = "Magic System/New Spell")]
	public class SpellDefinition : NodeGraph
	{
		private SpellRootNode _rootNode;

        public SpellRootNode GetRootNode{ get{
				if(_rootNode == null)
				{
					foreach(SpellNodeBase node in nodes)
					{
						if (node is SpellRootNode)
							_rootNode = node as SpellRootNode;
					}
				}
				return _rootNode;}}

		[SerializeField] public string spellName;
        [TextArea(0,5)]
		[SerializeField] public string spellDescription;
		public void UseAility(SpellController user)
		{
            GetRootNode?.UseAbility(user);
		}
	}
}