using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;

// Create custom ReadOnlyFloat attribute.
public class ReadOnlyFloatAttribute : PropertyAttribute
{

}
// Create custom ReadOnlyInt attribute.
public class ReadOnlyIntAttribute : PropertyAttribute
{

}
// Create custom ReadOnlyString attribute.
public class ReadOnlyStringAttribute : PropertyAttribute
{

}

// Modify custom ReadOnlyFloat attribute to display float fields as labels
// to ensure they are easily visible and unchangeable in inspector. 
[CustomPropertyDrawer(typeof(ReadOnlyFloatAttribute))]
public class ReadOnlyFloatDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        EditorGUI.LabelField(position, new GUIContent($"{property.displayName}: {property.floatValue}"));
    }
}
// Modify custom ReadOnlyFloat attribute to display int fields as labels
// to ensure they are easily visible and unchangeable in inspector. 
[CustomPropertyDrawer(typeof(ReadOnlyIntAttribute))]
public class ReadOnlyIntDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        EditorGUI.LabelField(position, new GUIContent($"{property.displayName}: {property.intValue}"));
    }
}
// Modify custom ReadOnlyFloat attribute to display int fields as labels
// to ensure they are easily visible and unchangeable in inspector. 
[CustomPropertyDrawer(typeof(ReadOnlyStringAttribute))]
public class ReadOnlyStringDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUILayout.Space(-20);
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(new GUIContent($"{property.displayName}:"));
            EditorGUILayout.LabelField(new GUIContent($"{property.stringValue}"));
        }
    }
}
