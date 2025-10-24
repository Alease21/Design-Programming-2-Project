using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

#if UNITY_EDITOR
namespace WFC
{
    [RequireComponent(typeof(Tilemap))]
    public class ItemTileModuleGenerator : MonoBehaviour
    {
        [SerializeField] private int _moduleWidth = 6;
        [SerializeField] private int _keyDepth = 1;

        [SerializeField] private List<ItemTileModule> _tileModules;

        public void GenerateTileModules()
        {
            _tileModules = new List<ItemTileModule>();

            Tilemap tilemap = GetComponent<Tilemap>();
            tilemap.CompressBounds();

            Vector2Int tileGridDimensions = new Vector2Int(tilemap.size.x / _moduleWidth, tilemap.size.y / _moduleWidth);

            for (int x = 0; x < tileGridDimensions.x; x++)
            {
                for (int y = 0; y < tileGridDimensions.y; y++)
                {
                    Vector3Int moduleOrigin = new Vector3Int(tilemap.origin.x + x * _moduleWidth, tilemap.origin.y + y * _moduleWidth);
                    List<TileBase> tiles = new List<TileBase>();

                    for (int j = 0; j < _moduleWidth; j++)
                    {
                        for (int i = 0; i < _moduleWidth; i++)
                        {
                            Vector3Int tilePos = moduleOrigin + new Vector3Int(i, j);
                            tiles.Add(tilemap.GetTile(tilePos));
                        }
                    }

                    ItemTileModule asset = ScriptableObject.CreateInstance<ItemTileModule>();
                    AssetDatabase.CreateAsset(asset, $"Assets/WFC SOs/Modules/ItemTileModules/Tile({x},{y}).asset");
                    AssetDatabase.SaveAssets();

                    asset.keyDepth = _keyDepth;
                    asset.moduleWidth = _moduleWidth;
                    asset.SetBasesAndKeys(tiles.ToArray());
                    EditorUtility.SetDirty(asset);

                    _tileModules.Add(asset);
                }
            }

            //SetModuleNeighbours();
        }
        public void ClearItemTileModule(ItemTileModule asset)
        {
            EditorUtility.ClearDirty(asset);
            DestroyImmediate(asset);
        }
    }
}
#endif