using UnityEditor;
using UnityEngine;

namespace WFC
{
    [CustomEditor(typeof(RoomSet))]
    public class RoomModuleSetEditor : Editor
    {
        private RoomSet _roomModuleSet;

        private void OnEnable()
        {
            _roomModuleSet = target as RoomSet;
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