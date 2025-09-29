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

                    //initial add every tile
                    north.Add(tileToEvaluate);
                    east.Add(tileToEvaluate);
                    south.Add(tileToEvaluate);
                    west.Add(tileToEvaluate);
                    
                    //shave options away
                    if (curTile.GetTileType == TileModule.TileType.Wall)
                    {
                        string curWallDirs = curTile.GetWallDirections();

                        if (tileToEvaluate.GetTileType == TileModule.TileType.Wall)
                        {
                            string wallDirsToEvaluate = tileToEvaluate.GetWallDirections();

                            if (curWallDirs[0] == 'N' && wallDirsToEvaluate[2] != 'S')
                                north.Remove(tileToEvaluate);
                            if (curWallDirs[1] == 'E' && wallDirsToEvaluate[3] != 'W')
                                east.Remove(tileToEvaluate);
                            if (curWallDirs[2] == 'S' && wallDirsToEvaluate[0] != 'N')
                                south.Remove(tileToEvaluate);
                            if (curWallDirs[3] == 'W' && wallDirsToEvaluate[1] != 'E')
                                west.Remove(tileToEvaluate);
                        }
                        /*
                        else
                        {
                            if (curWallDirs[0] == 'N')
                                north.Remove(tileToEvaluate);
                            if (curWallDirs[1] == 'E')
                                east.Remove(tileToEvaluate);
                            if (curWallDirs[2] == 'S')
                                south.Remove(tileToEvaluate);
                            if (curWallDirs[3] == 'W')
                                west.Remove(tileToEvaluate);
                        }*/
                    }
                }

                _tileModules[i].North = north.ToArray();
                _tileModules[i].East = east.ToArray();
                _tileModules[i].South = south.ToArray();
                _tileModules[i].West = west.ToArray();
            }
        }
    }
}
