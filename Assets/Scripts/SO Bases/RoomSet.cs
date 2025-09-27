using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using WFC;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New Room Module Set")]
    public class RoomSet : ScriptableObject, IModuleSet
    {
        private RoomModule[] _roomModules;

        public IModule[] Modules { get => _roomModules; set => value = _roomModules; }

        public void SetNeighbours()
        {
            for (int i = 0; i < _roomModules.Length; i++)
            {
                List<RoomModule> north = new List<RoomModule>();
                List<RoomModule> east = new List<RoomModule>();
                List<RoomModule> south = new List<RoomModule>();
                List<RoomModule> west = new List<RoomModule>();

                for (int j = 0; j < _roomModules.Length; j++)
                {
                    string curModuleDirections = _roomModules[i].GetDirString(); // set up to grab last 4 chars
                    string moduleToEvaluate = _roomModules[j].GetDirString();

                    if (curModuleDirections.Contains('N') && moduleToEvaluate.Contains('S') ||
                        curModuleDirections.Contains('N') == false && moduleToEvaluate.Contains('S') == false)
                    {
                        north.Add(_roomModules[j]);
                    }
                    if (curModuleDirections.Contains('E') && moduleToEvaluate.Contains('W') ||
                        curModuleDirections.Contains('E') == false && moduleToEvaluate.Contains('W') == false)
                    {
                        east.Add(_roomModules[j]);
                    }
                    if (curModuleDirections.Contains('S') && moduleToEvaluate.Contains('N')||
                        curModuleDirections.Contains('S') == false && moduleToEvaluate.Contains('N') == false)
                    {
                        south.Add(_roomModules[j]);
                    }
                    if (curModuleDirections.Contains('W') && moduleToEvaluate.Contains('E') ||
                        curModuleDirections.Contains('W') == false && moduleToEvaluate.Contains('E') == false)
                    {
                        west.Add(_roomModules[j]);
                    }
                }

                _roomModules[i].North = north.ToArray();
                _roomModules[i].East = east.ToArray();
                _roomModules[i].South = south.ToArray();
                _roomModules[i].West = west.ToArray();
            }
        }
    }
}