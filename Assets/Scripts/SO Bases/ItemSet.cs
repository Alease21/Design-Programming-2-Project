using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Module Sets/New Item Module Set")]
    public class ItemSet : ScriptableObject
    {
        public ItemModule[] itemModules;

        public void SetNeighbours()
        {
            for (int i = 0; i < itemModules.Length; i++)
            {
                List<ItemModule> north = new List<ItemModule>();
                List<ItemModule> east = new List<ItemModule>();
                List<ItemModule> south = new List<ItemModule>();
                List<ItemModule> west = new List<ItemModule>();

                for (int j = 0; j < itemModules.Length; j++)
                {
                    ItemModule.ItemType curItemtype = itemModules[i].itemType;
                    ItemModule.ItemType itemTypeToEval = itemModules[j].itemType;

                    switch ((int)curItemtype)
                    {
                        case 0: // None
                            north.Add(itemModules[j]);
                            east.Add(itemModules[j]);
                            south.Add(itemModules[j]);
                            west.Add(itemModules[j]);
                            break;
                        case 1:
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Box)
                            {
                                north.Add(itemModules[j]);
                                east.Add(itemModules[j]);
                                south.Add(itemModules[j]);
                                west.Add(itemModules[j]);
                            }
                            break;
                        case 2:// Table
                        case 3:// Chair
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Table ||
                                itemTypeToEval == ItemModule.ItemType.Chair)
                            {
                                north.Add(itemModules[j]);
                                east.Add(itemModules[j]);
                                south.Add(itemModules[j]);
                                west.Add(itemModules[j]);
                            }

                            break;
                        case 4:// Rocks
                            if (itemTypeToEval == ItemModule.ItemType.None)
                            {
                                north.Add(itemModules[j]);
                                east.Add(itemModules[j]);
                                south.Add(itemModules[j]);
                                west.Add(itemModules[j]);
                            }
                            break;
                        case 5:// torch
                        case 6:// Banner
                            if (itemTypeToEval == ItemModule.ItemType.None ||
                                itemTypeToEval == ItemModule.ItemType.Box ||
                                itemTypeToEval == ItemModule.ItemType.Rocks ||
                                itemTypeToEval == ItemModule.ItemType.Chair)
                            {
                                north.Add(itemModules[j]);
                                east.Add(itemModules[j]);
                                south.Add(itemModules[j]);
                                west.Add(itemModules[j]);
                            }
                            break;
                    }
                }

                itemModules[i].north = north.ToArray();
                itemModules[i].east = east.ToArray();
                itemModules[i].south = south.ToArray();
                itemModules[i].west = west.ToArray();
            }
        }
    }
}