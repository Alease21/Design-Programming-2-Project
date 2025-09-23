using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WFC;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New Room Module Set")]
    public class RoomModuleSet : ScriptableObject
    {
        public RoomModule[] roomModules;

        public void SetNeighbours()
        {
            for (int i = 0; i < roomModules.Length; i++)
            {
                List<RoomModule> north = new List<RoomModule>();
                List<RoomModule> east = new List<RoomModule>();
                List<RoomModule> south = new List<RoomModule>();
                List<RoomModule> west = new List<RoomModule>();

                for (int j = 0; j < roomModules.Length; j++)
                {
                    string curModuleDirections = roomModules[i].GetDirString(); // set up to grab last 4 chars
                    string moduleToEvaluate = roomModules[j].GetDirString();

                    if (curModuleDirections.Contains('N') && moduleToEvaluate.Contains('S') ||
                        curModuleDirections.Contains('N') == false && moduleToEvaluate.Contains('S') == false)
                    {
                        north.Add(roomModules[j]);
                    }
                    if (curModuleDirections.Contains('E') && moduleToEvaluate.Contains('W') ||
                        curModuleDirections.Contains('E') == false && moduleToEvaluate.Contains('W') == false)
                    {
                        east.Add(roomModules[j]);
                    }
                    if (curModuleDirections.Contains('S') && moduleToEvaluate.Contains('N')||
                        curModuleDirections.Contains('S') == false && moduleToEvaluate.Contains('N') == false)
                    {
                        south.Add(roomModules[j]);
                    }
                    if (curModuleDirections.Contains('W') && moduleToEvaluate.Contains('E') ||
                        curModuleDirections.Contains('W') == false && moduleToEvaluate.Contains('E') == false)
                    {
                        west.Add(roomModules[j]);
                    }
                }

                roomModules[i].north = north.ToArray();
                roomModules[i].east = east.ToArray();
                roomModules[i].south = south.ToArray();
                roomModules[i].west = west.ToArray();
            }
        }
    }
}