using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(EquipmentController)), CanEditMultipleObjects]
public class EquipmentControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        EquipmentController ec = target as EquipmentController;

        ec.SetEquippedWeapons();
        ec.SetEquippedArmor();

        var weapons = ec.GetWeapons;
        var armor = ec.GetArmor;

        List<string> invalidWeaps = new();
        List<string> invalidArmor = new();

        foreach (KeyValuePair<WeaponSOBase, bool> b in weapons)
            if (!b.Value) invalidWeaps.Add(b.Key.GetName);
        foreach (KeyValuePair<ArmorSOBase, bool> b in armor)
            if (!b.Value) invalidArmor.Add(b.Key.GetName);

        GUILayout.Label("Equipped Weapons", EditorStyles.boldLabel);
        if (invalidWeaps.Count > 0)
        {
            GUI.color = Color.red;
            GUILayout.Label($"*Invalid Weapon{(invalidWeaps.Count > 0 ? "s" : "")} Equipped*");
            EditorGUI.indentLevel++;
                foreach (string s in invalidWeaps)
                    GUILayout.Label($"\t{s}");
            EditorGUI.indentLevel--;
            GUI.color = Color.white;
        }
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_mainHand"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_offHand"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_weaponOffsetFromCenter"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_swingSpeed"));
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_physDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_iceDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_fireDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_holyDamage"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_unholyDamage"));
        EditorGUI.indentLevel--;

        GUILayout.Space(10);
        GUILayout.Label("Equipped Armor", EditorStyles.boldLabel);
        if (invalidArmor.Count > 0)
        {
            GUI.color = Color.red;
            GUILayout.Label("*Invalid Armor Equipped*");
            EditorGUI.indentLevel++;
                foreach (string s in invalidArmor)
                    GUILayout.Label($"\t{s}");
            EditorGUI.indentLevel--;
            GUI.color = Color.white;
        }
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_headArmor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_chestArmor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_legArmor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_footArmor"));
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_physResist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_iceResist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_fireResist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_holyResist"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_unholyResist"));
        EditorGUI.indentLevel--;


        serializedObject.ApplyModifiedProperties();
    }
}
