using UnityEngine;
using UnityEditor;

namespace WFC
{
    [CustomEditor(typeof(ItemTileSet))]
    public class ItemTileSetEditor : Editor
    {
        private ItemTileSet _itemTileSet;

        private void OnEnable()
        {
            _itemTileSet = target as ItemTileSet;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Neighbours"))
            {
                _itemTileSet.SetNeighbours();
            }
        }
    }
}