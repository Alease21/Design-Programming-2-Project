using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileModuleGenerator))]
public class TileModuleGenerator_Editor : Editor
{
    private TileModuleGenerator _cur;

    private void OnEnable()
    {
        _cur = (TileModuleGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Generate Tile Modules"))
        {
            _cur?.GenerateTileModules();
        }
    }
}