using MagicSystem;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(SpellRootNode))]
public class SpellRootNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        SpellRootNode node = target as SpellRootNode;
        NodePort targetingPort = node.GetPort("targeting");

        GUILayout.Label((node.graph as SpellDefinition).spellName, EditorStyles.boldLabel);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("targeting"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_manaCost"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_spellElement"));
        GUILayout.Space(5);

        if (targetingPort.IsConnected)
        {
            TargetingStrategy targetStrat = targetingPort.Connection.node as TargetingStrategy;
               
            if (CanBeHelpful(targetStrat))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("helpfulEffects"));
            if (CanBeHarmful(targetStrat))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("harmfulEffects"));
        }

        serializedObject.ApplyModifiedProperties();
    }
    private bool CanBeHelpful(TargetingStrategy strat)
    {
        return strat is SelfTarget || strat is ICanAffectOthers && (
             (strat as ICanAffectOthers).GetEffectType == EffectType.Helpful ||
             (strat as ICanAffectOthers).GetEffectType == EffectType.Both);
    }
    private bool CanBeHarmful(TargetingStrategy strat)
    {
        return strat is ICanAffectOthers && (
             (strat as ICanAffectOthers).GetEffectType == EffectType.Harmful ||
             (strat as ICanAffectOthers).GetEffectType == EffectType.Both);
    }
}
