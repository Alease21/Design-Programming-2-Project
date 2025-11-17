using MagicSystem;
using XNodeEditor;

[CustomNodeEditor(typeof(TargetingStrategy))]
public class TargetingNodeEditor : NodeEditor
{
    public override void OnBodyGUI()
    {
        serializedObject.Update();

        NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("input"));

        if (target is ICanAffectOthers)
        {
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_effectType"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_affectedLayers"));
            NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_range"));
            if (target is ProjectileTarget)
                NodeEditorGUILayout.PropertyField(serializedObject.FindProperty("_projectileSpeed"));
        }

        serializedObject.ApplyModifiedProperties();
    }
}
