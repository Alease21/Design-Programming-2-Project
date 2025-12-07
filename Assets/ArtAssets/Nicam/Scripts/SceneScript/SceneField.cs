using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

//NOT MINE (NICAM LIU)
//this data will be needed in game to direct our scenes
[System.Serializable]
public class SceneField
{
    [SerializeField] private Object sceneAsset;
    [SerializeField] private string sceneName = "";

    //getter, grabs data
    public string GetSceneName { get { return sceneName; } }

    //makes the object work with existing unity methods like LoadScene
    public static implicit operator string(SceneField sceneField)
    {
        return sceneField.GetSceneName;
    }
}

//property drawer  (User-Centered Design)
#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(SceneField))]
public class SceneFieldPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, GUIContent.none, property);
        SerializedProperty sceneAsset = property.FindPropertyRelative("sceneAsset");
        SerializedProperty sceneName = property.FindPropertyRelative("sceneName");
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        if (sceneAsset != null)
        {
            sceneAsset.objectReferenceValue = EditorGUI.ObjectField(position, sceneAsset.objectReferenceValue, typeof(SceneAsset), false);

            if (sceneAsset.objectReferenceValue != null)
            {
                sceneName.stringValue = (sceneAsset.objectReferenceValue as SceneAsset).name;
            }
        }

        EditorGUI.EndProperty();
    }
}
#endif
