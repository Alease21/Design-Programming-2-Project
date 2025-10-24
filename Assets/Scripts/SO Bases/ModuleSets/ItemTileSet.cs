using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New Item Tile Set")]
    public class ItemTileSet : ScriptableObject, IModuleSet
    {
        [SerializeField] private ItemTileModule[] _itemTileModules;
        [SerializeField] private int _moduleWidth;
        [SerializeField] private int _keyDepth;

        public IModule[] Modules { get => _itemTileModules; set => _itemTileModules = value as ItemTileModule[]; }
        public int GetTrueModuleWidth { get { return _moduleWidth - 2 * _keyDepth; } }
        public int GetModuleKeyDepth { get { return _keyDepth; } }

        public void SetNeighbours()
        {
            _moduleWidth = _itemTileModules[0].moduleWidth;
            _keyDepth = _itemTileModules[0].keyDepth;

            for (int j = 0; j < _itemTileModules.Length; j++)
            {
                ItemTileModule curModule = _itemTileModules[j];
                List<ItemTileModule> n = new(), e = new(), s = new(), w = new();

                for (int i = 0; i < _itemTileModules.Length; i++)
                {
                    ItemTileModule moduleToCompare = _itemTileModules[i];

                    if (curModule == moduleToCompare) continue;//same modules cannot be neighbors
                    
                    if (CheckKey(curModule.NorthKey, moduleToCompare.SouthKey))
                        n.Add(moduleToCompare);
                    if (CheckKey(curModule.EastKey, moduleToCompare.WestKey))
                        e.Add(moduleToCompare);
                    if (CheckKey(curModule.SouthKey, moduleToCompare.NorthKey))
                        s.Add(moduleToCompare);
                    if (CheckKey(curModule.WestKey, moduleToCompare.EastKey))
                        w.Add(moduleToCompare);
                }

                curModule.North = n.ToArray();
                curModule.East = e.ToArray();
                curModule.South = s.ToArray();
                curModule.West = w.ToArray();
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
}