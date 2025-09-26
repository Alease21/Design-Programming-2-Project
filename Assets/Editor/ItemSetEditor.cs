using UnityEngine;
using UnityEditor;

namespace WFC
{
    [CustomEditor(typeof(ItemSet))]
    public class ItemSetEditor : Editor
    {
        private ItemSet _itemSet;

        private void OnEnable()
        {
            _itemSet = target as ItemSet;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Neighbours"))
            {
                _itemSet.SetNeighbours();
            }
        }
    }
}