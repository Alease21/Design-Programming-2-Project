using MagicSystem;
using XNodeEditor;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

[CustomNodeEditor(typeof(EffectNodeBase))]
public class EffectNodeBaseEditor : NodeEditor
{
    private bool _showAffectedStats = false; // bool for foldout menu on buff/debuff nodes

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("input"));

        EffectNodeBase node = target as EffectNodeBase;
        string effectValueName = "Effect";

        if (node is HealEffect || node is DamageEffect)
        {
            bool isHeal = node is HealEffect ? true : false;
            effectValueName = isHeal ? "Heal" : "Damage";
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_healthValue"), new GUIContent($"{effectValueName} Value"));
        }
        else if (node is BuffEffect || node is DebuffEffect)
        {
            bool isBuff = node is BuffEffect ? true : false;
            effectValueName = isBuff ? "Buff" : "Debuff";
            node.isOverTime = true;
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_effectName"), new GUIContent($"{effectValueName} Name"));
            GUILayout.Space(10);
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_affectedStats"));

            IAuraEffect auraNode = node as IAuraEffect;
            if (!auraNode.GetAffectedStats.Equals(UnitStats.None)) // if no stats are affected, do not draw stat propertes foldout
            {
                _showAffectedStats = EditorGUILayout.Foldout(_showAffectedStats, $"{effectValueName} Values");
                if (_showAffectedStats)
                    DrawAffectedStatList(auraNode.GetAffectedStats);
                GUILayout.Space(5);
            }
        }

        if (node is not BuffEffect && node is not DebuffEffect)
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("isOverTime"));
        if (node.isOverTime)
        {
            EditorGUI.indentLevel++;
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_duration"));
            if (node is not IAuraEffect)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_numberOfTicks"), new GUIContent("# of Ticks"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
    private void DrawAffectedStatList(UnitStats statsEnum)
    {
        EditorGUI.indentLevel++;
        
        List<string> statTypes = new List<string>(statsEnum.ToString().Split(", "));
        if (statTypes.Contains("Stamina") || statTypes.Contains("-1"))
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_staminaValue"), new GUIContent($"Stamina"));
        if (statTypes.Contains("Strength") || statTypes.Contains("-1"))
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_strengthValue"), new GUIContent($"Strength"));
        if (statTypes.Contains("Dexterity") || statTypes.Contains("-1"))
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_dexterityValue"), new GUIContent($"Dexterity"));
        if (statTypes.Contains("Intelligence") || statTypes.Contains("-1"))
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_intelligenceValue"), new GUIContent($"Intelligence"));

        EditorGUI.indentLevel--;
    }
}
