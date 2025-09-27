using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New TileSet")]
    public class TileSet : ScriptableObject, IModuleSet
    {
        public TileModule[] _tileModules;

        public IModule[] Modules { get => _tileModules; set => value = _tileModules; }

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
                    north.Add(_tileModules[j]);
                    east.Add(_tileModules[j]);
                    south.Add(_tileModules[j]);
                    west.Add(_tileModules[j]);

                    /* Failed Attempt @ setting neighbours for tiles
                    TileModule curTile = tileModules[i];
                    TileModule tileToEvaluate = tileModules[j];

                    if (curTile.tileType == TileModule.TileType.Wall)
                    {
                        string curTileDirections = curTile.GetWallDirections(); // set up to grab last 4 of string

                        if (tileToEvaluate.tileType == TileModule.TileType.Wall)
                        {
                            string stringToEvaluate = tileToEvaluate.GetWallDirections();

                            if (curTileDirections.Contains('N') && stringToEvaluate.Contains('S'))
                            {
                                north.Add(tileToEvaluate);
                            }
                            if (curTileDirections.Contains('E') && stringToEvaluate.Contains('W'))
                            {
                                east.Add(tileToEvaluate);
                            }
                            if (curTileDirections.Contains('S') && stringToEvaluate.Contains('N'))
                            {
                                south.Add(tileToEvaluate);
                            }
                            if (curTileDirections.Contains('W') && stringToEvaluate.Contains('E'))
                            {
                                west.Add(tileToEvaluate);
                            }
                        }
                        else
                        {
                            if (!curTileDirections.Contains('N'))
                            {
                                north.Add(tileToEvaluate);
                            }
                            if (!curTileDirections.Contains('E'))
                            {
                                east.Add(tileToEvaluate);
                            }
                            if (!curTileDirections.Contains('S'))
                            {
                                south.Add(tileToEvaluate);
                            }
                            if (!curTileDirections.Contains('W'))
                            {
                                west.Add(tileToEvaluate);
                            }
                        }
                    }

                    if (curTile.tileType == TileModule.TileType.Pit)
                    {
                        char curTileSubtype = curTile.GetTileSubType();
                        char tileTypeToEvaluate = tileToEvaluate.GetTileSubType();

                        if (curTileSubtype == '0')
                        {
                            if (tileToEvaluate.tileType == TileModule.TileType.Pit && 
                                tileTypeToEvaluate == '1')
                            {
                                south.Add(tileToEvaluate);
                            }
                            else if (tileToEvaluate.tileType == TileModule.TileType.Pit &&
                                tileTypeToEvaluate == '0')
                            {
                                east.Add(tileToEvaluate);
                                west.Add(tileToEvaluate);
                            }
                            else
                            {
                                north.Add(tileToEvaluate);
                                east.Add(tileToEvaluate);
                                west.Add(tileToEvaluate);
                            }
                        }
                        else if (curTileSubtype == '1')
                        {
                            if (tileToEvaluate.tileType == TileModule.TileType.Pit &&
                                tileTypeToEvaluate == '0')
                            {
                                north.Add(tileToEvaluate);
                            }
                            else
                            {
                                east.Add(tileToEvaluate);
                                south.Add(tileToEvaluate);
                                west.Add(tileToEvaluate);
                            }
                        }
                    }

                    if (curTile.tileType == TileModule.TileType.Floor)
                    {
                        char curTileSubtype = curTile.GetTileSubType();
                        char tileTypeToEvaluate = tileToEvaluate.GetTileSubType();

                        switch (curTileSubtype)
                        {
                            case '0'://No Cracks
                                north.Add(tileToEvaluate);
                                east.Add(tileToEvaluate);
                                south.Add(tileToEvaluate);
                                west.Add(tileToEvaluate);
                                break;
                            case '1': //Med Crack
                                if (tileToEvaluate.tileType == TileModule.TileType.Floor)
                                {
                                    if (tileTypeToEvaluate == '0' ||
                                        tileTypeToEvaluate == '5')
                                    {
                                        north.Add(tileToEvaluate);
                                    }
                                }
                                else
                                {
                                    east.Add(tileToEvaluate);
                                    south.Add(tileToEvaluate);
                                    west.Add(tileToEvaluate);
                                }
                                break;
                            case '2': //Small crack
                                north.Add(tileToEvaluate);
                                east.Add(tileToEvaluate);
                                south.Add(tileToEvaluate);
                                west.Add(tileToEvaluate);
                                break;
                            case '3': //Dirt Corner
                                if (tileToEvaluate.tileType == TileModule.TileType.Wall ||
                                    tileToEvaluate.tileType == TileModule.TileType.Pit)
                                {
                                    north.Add(tileToEvaluate);
                                    west.Add(tileToEvaluate);
                                }
                                else
                                {
                                    south.Add(tileToEvaluate);
                                    east.Add(tileToEvaluate);
                                }
                                break;
                            case '4':
                                //not implemented currently
                                break;
                            case '5': //Big Crack
                                if (tileToEvaluate.tileType == TileModule.TileType.Floor)
                                {
                                    if (tileTypeToEvaluate == '1' ||
                                        tileToEvaluate.tileType != TileModule.TileType.Floor)
                                    {
                                        south.Add(tileToEvaluate);
                                    }
                                    else
                                    {
                                        north.Add(tileToEvaluate);
                                        east.Add(tileToEvaluate);
                                        west.Add(tileToEvaluate);
                                    }
                                }
                                break;
                        }
                    }*/
                }

                _tileModules[i].North = north.ToArray();
                _tileModules[i].East = east.ToArray();
                _tileModules[i].South = south.ToArray();
                _tileModules[i].West = west.ToArray();
            }
        }
    }
}
