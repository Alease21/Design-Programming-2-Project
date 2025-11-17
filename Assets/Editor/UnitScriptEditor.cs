using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UnitScript)), CanEditMultipleObjects]
public class UnitScriptEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.UpdateIfRequiredOrScript();

        UnitScript unitScript = (UnitScript)target;
        unitScript.UpdateClassInfoInspector();

        GUILayout.Label("Unit Info", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_unitType"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_characterClass"));
            if (unitScript.GetUnitType != UnitTypes.Player)
                EditorGUILayout.PropertyField(serializedObject.FindProperty("_expGiveOnDeath"));
            GUILayout.Space(10);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedWeapons"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedArmor"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_allowedMagic"));
        EditorGUI.indentLevel--;

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //line separator

        using (new EditorGUILayout.HorizontalScope())
        {
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("Runtime Unit Stats", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_level"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_curExp"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxExpToLevel"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_health"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_mana"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_adjustedStamina"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_adjustedStrength"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_adjustedDexterity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_adjustedIntelligence"));
                EditorGUI.indentLevel--;
            }
            using (new EditorGUILayout.VerticalScope())
            {
                GUILayout.Label("Base Unit Stats", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_initLevel"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_initMaxExpToLevel"));
                    GUILayout.Space(20);
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxHealth"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_maxMana"));

                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseStamina"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseStrength"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseDexterity"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("_baseIntelligence"));
                EditorGUI.indentLevel--;
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider); //line separator

        serializedObject.ApplyModifiedProperties();
    }
}
