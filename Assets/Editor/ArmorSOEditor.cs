using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(ArmorSOBase)), CanEditMultipleObjects]
public class ArmorSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        DrawDefaultInspector();

        ArmorSOBase armorSO = (ArmorSOBase)target;
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_armorResistances"));

        EditorGUI.indentLevel++;
            List<string> dmgTypes = new List<string>(armorSO.GetDamageResistTypes.ToString().Split(", "));
            if (dmgTypes.Contains("Physical") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_physResist"));
            if (dmgTypes.Contains("Ice") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_iceResist"));
            if (dmgTypes.Contains("Fire") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_fireResist"));
            if (dmgTypes.Contains("Holy") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_holyResist"));
            if (dmgTypes.Contains("Unholy") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_unholyResist"));
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
