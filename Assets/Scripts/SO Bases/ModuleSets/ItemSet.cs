using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New Item Module Set")]
    public class ItemSet : ScriptableObject, IModuleSet
    {
        [SerializeField] private ItemModule[] _itemModules;
        public IModule[] Modules { get => _itemModules; set => _itemModules = value as ItemModule[]; }

        public void SetNeighbours()
        {
            for (int i = 0; i < _itemModules.Length; i++)
            {
                List<ItemModule> north = new List<ItemModule>();
                List<ItemModule> east = new List<ItemModule>();
                List<ItemModule> south = new List<ItemModule>();
                List<ItemModule> west = new List<ItemModule>();

                for (int j = 0; j < _itemModules.Length; j++)
                {
                    ItemModule.ItemType curItemtype = _itemModules[i].GetItemType;
                    ItemModule.ItemType itemTypeToEval = _itemModules[j].GetItemType;

                    switch ((int)curItemtype)
                    {
                        case 0: // None
                            north.Add(_itemModules[j]);
                            east.Add(_itemModules[j]);
                            south.Add(_itemModules[j]);
                            west.Add(_itemModules[j]);
                            break;
                        case 1:// Box
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Table)
                            {
                                north.Add(_itemModules[j]);
                                east.Add(_itemModules[j]);
                                south.Add(_itemModules[j]);
                                west.Add(_itemModules[j]);
                            }
                            break;
                        case 2:// Table
                            if (itemTypeToEval == ItemModule.ItemType.None || 
                                itemTypeToEval == ItemModule.ItemType.Chair)
                            {
                                north.Add(_itemModules[j]);
                                east.Add(_itemModules[j]);
                                south.Add(_itemModules[j]);
                                west.Add(_itemModules[j]);
                            }
                            break;
                        case 3:// Chair
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Table)
                            {
                                north.Add(_itemModules[j]);
                                east.Add(_itemModules[j]);
                                south.Add(_itemModules[j]);
                                west.Add(_itemModules[j]);
                            }
                            break;
                        case 4:// Rocks
                            if (itemTypeToEval == ItemModule.ItemType.None)
                            {
                                north.Add(_itemModules[j]);
                                east.Add(_itemModules[j]);
                                south.Add(_itemModules[j]);
                                west.Add(_itemModules[j]);
                            }
                            break;
                        case 5:// torch
                        case 6:// Banner
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Box ||
                                itemTypeToEval == ItemModule.ItemType.Rocks ||
                                itemTypeToEval == ItemModule.ItemType.Chair)
                            {
                                north.Add(_itemModules[j]);
                                east.Add(_itemModules[j]);
                                south.Add(_itemModules[j]);
                                west.Add(_itemModules[j]);
                            }
                            break;
                    }
                    //
                }
                _itemModules[i].North = north.ToArray();
                _itemModules[i].East = east.ToArray();
                _itemModules[i].South = south.ToArray();
                _itemModules[i].West = west.ToArray();
            }
        }
    }
}