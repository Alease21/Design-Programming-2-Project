using AbilitySystem;
using UnityEditor;
using UnityEngine;
using XNode;
using XNodeEditor;

[CustomNodeEditor(typeof(AbilityRootNode))]
public class AbilityRootNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        AbilityRootNode node = target as AbilityRootNode;
        NodePort targetingPort = node.GetPort("targeting");

        GUILayout.Label((node.graph as AbilityDefinition).spellName, EditorStyles.boldLabel);
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("targeting"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_abilityCD"));
        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_abilityType"));
        GUILayout.Space(5);

        if (targetingPort.IsConnected)
        {
            TargetingStrategy targetStrat = targetingPort.Connection.node as TargetingStrategy;
               
            if (CanBeHelpful(targetStrat))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("helpfulEffects"));
            if (CanBeHarmful(targetStrat))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("harmfulEffects"));
            if (CanBeMisc(targetStrat))
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("miscEffects"));
        }

        serializedObject.ApplyModifiedProperties();
    }
    private bool CanBeHelpful(TargetingStrategy strat)
    {
        return strat is SelfTarget || strat is ICanAffectOthers && (
             (strat as ICanAffectOthers).GetEffectType == EffectType.Helpful ||
             (strat as ICanAffectOthers).GetEffectType == EffectType.All);
    }
    private bool CanBeHarmful(TargetingStrategy strat)
    {
        return strat is ICanAffectOthers && (
             (strat as ICanAffectOthers).GetEffectType == EffectType.Harmful ||
             (strat as ICanAffectOthers).GetEffectType == EffectType.All);
    }
    private bool CanBeMisc(TargetingStrategy strat)
    {
        return strat is ICanAffectOthers && (
             (strat as ICanAffectOthers).GetEffectType == EffectType.Misc ||
             (strat as ICanAffectOthers).GetEffectType == EffectType.All);
    }
}
