using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MultiplayerObjBase), true)]
public class MultiplayerObjEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var obj = target as MultiplayerObjBase;
        string guid = obj.GetGUID == "" ? "NO GUID SET" : obj.GetGUID;

        GUILayout.Label($"GUID: ( {guid} )");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //line seperator

        base.OnInspectorGUI();
    }
}
