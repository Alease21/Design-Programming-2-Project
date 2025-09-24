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
                    string curModuleDirections = tileModules[i].GetDirString(); // set up to grab last 4 chars
                    string moduleToEvaluate = tileModules[j].GetDirString();

                    if (curModuleDirections.Contains('N') && moduleToEvaluate.Contains('S') ||
                        curModuleDirections.Contains('N') == false && moduleToEvaluate.Contains('S') == false)
                    {
                        north.Add(tileModules[j]);
                    }
                    if (curModuleDirections.Contains('E') && moduleToEvaluate.Contains('W') ||
                        curModuleDirections.Contains('E') == false && moduleToEvaluate.Contains('W') == false)
                    {
                        east.Add(tileModules[j]);
                    }
                    if (curModuleDirections.Contains('S') && moduleToEvaluate.Contains('N') ||
                        curModuleDirections.Contains('S') == false && moduleToEvaluate.Contains('N') == false)
                    {
                        south.Add(tileModules[j]);
                    }
                    if (curModuleDirections.Contains('W') && moduleToEvaluate.Contains('E') ||
                        curModuleDirections.Contains('W') == false && moduleToEvaluate.Contains('E') == false)
                    {
                        west.Add(tileModules[j]);
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
