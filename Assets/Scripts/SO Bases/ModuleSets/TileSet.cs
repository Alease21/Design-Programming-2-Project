using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New TileSet")]
    public class TileSet : ScriptableObject, IModuleSet
    {
        [SerializeField] private TileModule[] _tileModules;
        public IModule[] Modules { get => _tileModules; set => _tileModules = value as TileModule[]; }

        public void SetNeighbours()
        {
            for (int i = 0; i < _tileModules.Length; i++)
            {
                List<TileModule> north = new List<TileModule>();
                List<TileModule> east = new List<TileModule>();
                List<TileModule> south = new List<TileModule>();
                List<TileModule> west = new List<TileModule>();

                for (int j = 0; j < _tileModules.Length; j++)
                {
                    TileModule curTile = _tileModules[i];
                    TileModule tileToEvaluate = _tileModules[j];

                    north.Add(tileToEvaluate);
                    east.Add(tileToEvaluate);
                    south.Add(tileToEvaluate);
                    west.Add(tileToEvaluate);
                }

                _tileModules[i].North = north.ToArray();
                _tileModules[i].East = east.ToArray();
                _tileModules[i].South = south.ToArray();
                _tileModules[i].West = west.ToArray();
            }
        }
    }
}
