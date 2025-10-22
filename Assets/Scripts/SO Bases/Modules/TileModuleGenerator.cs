using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR

[RequireComponent(typeof(Tilemap))]
public class TileModuleGenerator : MonoBehaviour
{
    [SerializeField] private int moduleWidth = 6;
    [SerializeField] private int keyDepth = 1;

    private List<TileModule> tileModules = new List<TileModule>();

    public void GenerateTileModules()
    {
        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        Vector2Int tileGridDimensions = new Vector2Int(tilemap.size.x / moduleWidth, tilemap.size.y / moduleWidth);

        int width = moduleWidth - keyDepth;
        int height = moduleWidth - keyDepth;

        for (int y = 0; y < tileGridDimensions.y; y++)
        {
            for (int x = 0; x < tileGridDimensions.x; x++)
            {
                Vector3Int moduleOrigin = new Vector3Int(tilemap.origin.x + x * moduleWidth, tilemap.origin.y + y * moduleWidth);
                List<TileBase> tiles = new List<TileBase>();

                for (int i = keyDepth - 1; i < width; i++)
                {
                    for (int j = keyDepth - 1; j < height; j++)
                    {
                        Vector3Int newPos = moduleOrigin + new Vector3Int(x * moduleWidth + i, y * moduleWidth + j);
                        tiles.Add(tilemap.GetTile(newPos));
                    }
                }

                TileModule asset = ScriptableObject.CreateInstance<TileModule>();
                AssetDatabase.CreateAsset(asset, $"Assets/WFC SOs/Modules/TileModules/Tile({x},{y}).asset");
                AssetDatabase.SaveAssets();

                asset.keyDepthByModuleWidth = new Vector2Int(keyDepth, moduleWidth);
                asset.SetBasesAndKeys(tiles.ToArray());
                EditorUtility.SetDirty(asset);

                tileModules.Add(asset);
            }
        }

        SetModuleNeighbours();
    }

    public void SetModuleNeighbours()
    {
        for (int i = 0; i < tileModules.Count; i++)
        {
            TileModule curModule = tileModules[i];
            List<TileModule> n = new(), e = new(), s = new(), w = new();

            for (int j = 0; j < tileModules.Count; j++)
            {
                TileModule moduleToCompare = tileModules[j];

                if (CheckKey(curModule.nKey, moduleToCompare.nKey))
                    n.Add(moduleToCompare);
                if (CheckKey(curModule.eKey, moduleToCompare.eKey))
                    e.Add(moduleToCompare);
                if (CheckKey(curModule.sKey, moduleToCompare.sKey))
                    s.Add(moduleToCompare);
                if (CheckKey(curModule.wKey, moduleToCompare.wKey))
                    w.Add(moduleToCompare);
            }

            curModule.northNeighbours = n.ToArray();
            curModule.eastNeighbours = e.ToArray();
            curModule.southNeighbours = s.ToArray();
            curModule.westNeighbours = w.ToArray();
        }
    }
    public bool CheckKey(TileBase[] key1, TileBase[] key2)
    {
        if (key1.Length != key2.Length)
        {
            Debug.LogError("Key Length not equal");
            return false;
        }

        for (int i = 0; i < key1.Length; i++)
            if (key1[i] != key2[i]) return false;
        return true;
    }
}
#endif