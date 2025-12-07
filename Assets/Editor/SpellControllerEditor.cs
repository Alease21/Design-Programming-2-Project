using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

//[CustomEditor(typeof(SpellController))]
public class SpellControllerEditor : Editor 
{
    /*
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        SpellController sc = target as SpellController;

        if (sc.SetSpells() == null || sc.GetUnit.GetCharacterClass == null)
        {
            GUILayout.Label(new GUIContent("* UNIT CLASS NOT SET *"));
            return;
        }

        var spells = sc.GetSpells;

        List<string> invalidSpells = new();

        foreach (KeyValuePair<SpellDefinition, bool> b in spells)
            if (!b.Value) invalidSpells.Add(b.Key.name);

        GUILayout.Label("Equipped Spells", EditorStyles.boldLabel);
        if (invalidSpells.Count > 0)
        {
            GUI.color = Color.red;
            GUILayout.Label($"*Invalid Spell{(invalidSpells.Count > 0 ? "s" : "")} Equipped*");
            EditorGUI.indentLevel++;
            foreach (string s in invalidSpells)
                GUILayout.Label($"\t{s}");
            EditorGUI.indentLevel--;
            GUI.color = Color.white;
        }
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spell1"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spell2"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spell3"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_spell4"));
        EditorGUI.indentLevel--;

        serializedObject.ApplyModifiedProperties();
    }
    */
}
