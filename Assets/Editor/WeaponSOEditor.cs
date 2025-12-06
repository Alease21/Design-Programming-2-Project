using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

[CustomEditor(typeof(WeaponSOBase)), CanEditMultipleObjects]
public class WeaponSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();
        DrawDefaultInspector();

        WeaponSOBase weaponSO = (WeaponSOBase)target;
        GUILayout.Space(10);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_damageTypes"));

        EditorGUI.indentLevel++;
            List<string> dmgTypes = new List<string>(weaponSO.GetDamageTypes.ToString().Split(", "));
            if (dmgTypes.Contains("Physical") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_physDamage"));
            if (dmgTypes.Contains("Ice") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_iceDamage"));
            if (dmgTypes.Contains("Fire") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_fireDamage"));
            if (dmgTypes.Contains("Holy") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_holyDamage"));
            if (dmgTypes.Contains("Unholy") || dmgTypes.Contains("-1"))
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_unholyDamage"));
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
}
