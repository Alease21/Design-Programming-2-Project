using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR

[RequireComponent(typeof(Tilemap))]
public class ItemTileModuleGenerator : MonoBehaviour
{
    [SerializeField] private int moduleWidth = 6;
    [SerializeField] private int keyDepth = 1;

    [SerializeField] private List<ItemTileModule> tileModules;

    public void GenerateTileModules()
    {
        tileModules = new List<ItemTileModule>();

        Tilemap tilemap = GetComponent<Tilemap>();
        tilemap.CompressBounds();
        
        Vector2Int tileGridDimensions = new Vector2Int(tilemap.size.x / moduleWidth, tilemap.size.y / moduleWidth);
        
        for (int x = 0; x < tileGridDimensions.x; x++)
        {
            for (int y = 0; y < tileGridDimensions.y; y++)
            {
                Vector3Int moduleOrigin = new Vector3Int(tilemap.origin.x + x * moduleWidth, tilemap.origin.y + y * moduleWidth);
                List<TileBase> tiles = new List<TileBase>();

                for (int j = 0; j < moduleWidth; j++)
                {
                    for (int i = 0; i < moduleWidth; i++)
                    {
                        Vector3Int tilePos = moduleOrigin + new Vector3Int(i,j);
                        tiles.Add(tilemap.GetTile(tilePos));
                    }
                }

                ItemTileModule asset = ScriptableObject.CreateInstance<ItemTileModule>();
                AssetDatabase.CreateAsset(asset, $"Assets/WFC SOs/Modules/ItemTileModules/Tile({x},{y}).asset");
                AssetDatabase.SaveAssets();

                asset.keyDepthByModuleWidth = new Vector2Int(keyDepth, moduleWidth);
                asset.SetBasesAndKeys(tiles.ToArray());
                EditorUtility.SetDirty(asset);

                tileModules.Add(asset);
            }
        }

        SetModuleNeighbours();
    }
    public void ClearItemTileModule(ItemTileModule asset)
    {
        EditorUtility.ClearDirty(asset);
        DestroyImmediate(asset);
    }
    public void SetModuleNeighbours()
    {
        for (int j = 0; j < tileModules.Count; j++)
        {
            ItemTileModule curModule = tileModules[j];
            List<ItemTileModule> n = new(), e = new(), s = new(), w = new();

            for (int i = 0; i < tileModules.Count; i++)
            {
                ItemTileModule moduleToCompare = tileModules[i];

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
        if (key1 == null || key2 == null)
        {
            Debug.LogError("Key is null");
            return false;
        }
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