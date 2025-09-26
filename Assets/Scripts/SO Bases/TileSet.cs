using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New TileSet")]
    public class TileSet : ScriptableObject
    {
        public TileModule[] tileModules;

        public void SetNeighbours()
        {
            for (int i = 0; i < tileModules.Length; i++)
            {
                List<TileModule> north = new List<TileModule>();
                List<TileModule> east = new List<TileModule>();
                List<TileModule> south = new List<TileModule>();
                List<TileModule> west = new List<TileModule>();

                for (int j = 0; j < tileModules.Length; j++)
                {

                    if (tileModules[i].tileType == TileModule.TileType.Wall &&
                        tileModules[j].tileType == TileModule.TileType.Wall)
                    {
                        string curModuleDirections = tileModules[i].GetWallDirections(); // set up to grab last 4 chars
                        string moduleToEvaluate = tileModules[j].GetWallDirections();

                        if (curModuleDirections.Contains('N') && moduleToEvaluate.Contains('S'))
                        {
                                north.Add(tileModules[j]); 
                        }
                        if (curModuleDirections.Contains('E') && moduleToEvaluate.Contains('W'))
                        {
                                east.Add(tileModules[j]);
                        }
                        if (curModuleDirections.Contains('S') && moduleToEvaluate.Contains('N'))
                        {
                                south.Add(tileModules[j]);
                        }
                        if (curModuleDirections.Contains('W') && moduleToEvaluate.Contains('E'))
                        {
                                west.Add(tileModules[j]);
                        }
                    }
                    else if (tileModules[i].tileType == TileModule.TileType.Pit)
                    {
                        char curTileSubType = tileModules[i].GetTileSubType();
                        char TileSubTypeToEval = tileModules[j].GetTileSubType();

                        if (curTileSubType == '0')
                        {
                            east.Add(tileModules[j]);
                            west.Add(tileModules[j]);
                            if (tileModules[j].tileType != TileModule.TileType.Pit)
                            {
                                north.Add(tileModules[j]);
                            }
                            if (tileModules[j].tileType == TileModule.TileType.Pit &&
                                TileSubTypeToEval == '1')
                            {
                                south.Add(tileModules[j]);
                            }
                        }
                        else if (curTileSubType == '1')
                        {
                            east.Add(tileModules[j]);
                            west.Add(tileModules[j]);

                            if (tileModules[j].tileType != TileModule.TileType.Pit)
                            {
                                south.Add(tileModules[j]);
                            }

                            if (tileModules[j].tileType == TileModule.TileType.Pit &&
                                TileSubTypeToEval == '0')
                            {
                                north.Add(tileModules[j]);
                            }
                        }
                    }
                    else if (tileModules[i].tileType == TileModule.TileType.Floor)
                    {
                        if (tileModules[j].tileType != TileModule.TileType.Wall)
                        {
                            east.Add(tileModules[j]);
                            if (tileModules[j].tileType == TileModule.TileType.Pit)
                            {
                                if (tileModules[j].GetTileSubType() != '0')
                                {
                                    south.Add(tileModules[j]);
                                }
                                else if (tileModules[j].GetTileSubType() != '1')
                                {
                                    north.Add(tileModules[j]);
                                }
                            }
                            else
                            {
                                north.Add(tileModules[j]);
                                south.Add(tileModules[j]);
                            }
                            west.Add(tileModules[j]);
                        }
                    }
                }

                tileModules[i].north = north.ToArray();
                tileModules[i].east = east.ToArray();
                tileModules[i].south = south.ToArray();
                tileModules[i].west = west.ToArray();
            }
        }
    }
}
