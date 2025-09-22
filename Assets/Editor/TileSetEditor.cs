using UnityEditor;
using UnityEngine;

namespace WFC
{
    [CustomEditor(typeof(RoomModuleSet))]
    public class TileSetEditor : Editor
    {
        private RoomModuleSet _roomModuleSet;

        private void OnEnable()
        {
            _roomModuleSet = target as RoomModuleSet;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Neighbours"))
            {
                _roomModuleSet.SetNeighbours();
            }
        }
    }
}