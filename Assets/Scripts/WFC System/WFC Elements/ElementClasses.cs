using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using static WFC.AStarPathfinding;

namespace WFC
{
    public class RoomElement : ElementBase
    {
        private bool _isTruePath = false;
        private Tilemap _selectedRoomPrefab;
        private byte[,] _roomByteMap;

        public bool IsTruePath { get { return _isTruePath; } set { _isTruePath = value; } }
        public Tilemap GetSelectedRoomPrefab { get { return _selectedRoomPrefab; } }
        public byte[,] GetRoomByteMap { get { return _roomByteMap; } }

        public RoomElement(IModule[] options, Vector2Int position)
        {
            _options = new List<IModule>(options);
            _position = position;

            if (_position.x == 0 || _position.x == DungeonCreator.instance.GetMapSize.x - 1 ||
                _position.y == 0 || _position.y == DungeonCreator.instance.GetMapSize.y - 1)
            {
                _isEdge = true;
            }
            else _isEdge = false;
        }

        public override void Collapse()
        {

            RemoveOptionsFromPosition();

            int rng = Random.Range(0, _options.Count);
            _selectedModule = _options[rng];

            //set tilemap prefab for ref in item WFC
            rng = Random.Range(0, (_selectedModule as RoomModule).GetRoomPrefabs.Length);
            _selectedRoomPrefab = (_selectedModule as RoomModule).GetRoomPrefabs[rng];

            Vector2Int mapSize = DungeonCreator.instance.GetMapSize;
            Vector2Int roomSize = DungeonCreator.instance.GetRoomSize;
            _roomByteMap = new byte[roomSize.x, roomSize.y];
            List<Vector2Int> exitLocations = new();

            for (int x = 0; x < roomSize.x; x++)
            {
                for (int y = 0; y < roomSize.y; y++)
                {
                    if (_selectedRoomPrefab.GetTile(new Vector3Int(x - roomSize.x / 2, y - roomSize.y / 2)).name[0] == 'P') //If tile is pit set to 2
                        _roomByteMap[x, y] = 2;
                    else if (_selectedRoomPrefab.GetTile(new Vector3Int(x - roomSize.x / 2, y - roomSize.y / 2)).name[0] == 'W')//If tile is wall set to 1
                        _roomByteMap[x, y] = 1;
                    else //If tile is floor set to 0
                        _roomByteMap[x, y] = 0;

                    Vector2Int tilePos = new Vector2Int(x, y);

                    if (DetermineIfEnterExitTile(tilePos))
                    {
                        if (_position.x == 0 && tilePos.x == 0 || _position.x == mapSize.x - 1 && tilePos.x == roomSize.x - 1 ||
                            _position.y == 0 && tilePos.y == 0 || _position.y == mapSize.y - 1 && tilePos.y == roomSize.y - 1)
                        {
                            _roomByteMap[x, y] = 4;//Tile is entrance or exit
                        }
                        exitLocations.Add(tilePos);
                    }
                }
            }

            //A* pathfinding to flip true path byte to 3
            _roomByteMap = FindTruePathThroughRoom(_roomByteMap, exitLocations.ToArray());
        }

        //Determine if a tile is an entrance or exit for byte flipping in Collapse method
        private bool DetermineIfEnterExitTile(Vector2Int tilePos)
        {
            Vector2Int roomSize = DungeonCreator.instance.GetRoomSize;
            string roomDirs = (_selectedModule as RoomModule).GetDirString();

            if ((roomDirs[3] == 'W' && tilePos.x == 0 && (tilePos.y == roomSize.y / 2 || tilePos.y == roomSize.y / 2 - 1)) ||
                (roomDirs[1] == 'E' && tilePos.x == roomSize.x - 1 && (tilePos.y == roomSize.y / 2 || tilePos.y == roomSize.y / 2 - 1)) ||
                (roomDirs[2] == 'S' && tilePos.y == 0 && (tilePos.x == roomSize.x / 2 || tilePos.x == roomSize.x / 2 - 1)) ||
                (roomDirs[0] == 'N' && tilePos.y == roomSize.y - 1 && (tilePos.x == roomSize.x / 2 || tilePos.x == roomSize.x / 2 - 1)))
                return true;

            return false;
        }
        public override void RemoveOptionsFromPosition()
        {
            DungeonCreator dc = DungeonCreator.instance;
            float distFromStart = 0f;

            //Remove any options that lead out of bounds unless setting entrance/exit
            if (_isEdge)
            {
                if (dc.IsStartMade)
                    distFromStart = (float)(_position - dc.StartRoom.GetPosition).magnitude;

                char edgeNS = 'Z';
                char edgeEW = 'Z';

                if (_position.x == 0)
                    edgeEW = 'W';
                else if (_position.x == dc.GetMapSize.x - 1)
                    edgeEW = 'E';
                if (_position.y == 0)
                    edgeNS = 'S';
                else if (_position.y == dc.GetMapSize.y - 1)
                    edgeNS = 'N';

                if (!dc.IsStartMade ||
                    !dc.IsExitMade && distFromStart > 0.65f * dc.GetMapSize.magnitude) // 0.65f just a magic number for min dist an exit can be made
                                                                                       // (make variable at some point)
                {
                    if (!dc.IsStartMade)
                    {
                        dc.IsStartMade = true;
                        dc.StartRoom = this;
                    }
                    else if (!dc.IsExitMade)
                    {
                        dc.IsExitMade = true;
                        dc.ExitRoom = this;
                    }
                }

                for (int i = _options.Count - 1; i >= 0; i--)
                {
                    RoomModule option = _options[i] as RoomModule;
                    string curModuleDirections = option.GetDirString();
                    int dirCounter = 0;

                    foreach (char dir in curModuleDirections)
                    {
                        if (dir != '-')
                            dirCounter++;
                    }

                    if (dc.StartRoom == this || dc.ExitRoom == this)
                    {
                        if (curModuleDirections.Contains(edgeNS) && curModuleDirections.Contains(edgeEW) ||
                            (edgeNS != 'Z' && !curModuleDirections.Contains(edgeNS) && edgeEW != 'Z' && !curModuleDirections.Contains(edgeEW)) ||
                            (edgeNS != 'Z' && !curModuleDirections.Contains(edgeNS) && edgeEW == 'Z') ||
                            (edgeNS == 'Z' && edgeEW != 'Z' && !curModuleDirections.Contains(edgeEW)))
                        {
                            _options.RemoveAt(i);
                        }
                    }
                    else
                    {
                        if ((edgeNS != 'Z' && curModuleDirections.Contains(edgeNS)) || (edgeEW != 'Z' &&
                            curModuleDirections.Contains(edgeEW)))
                        {
                            _options.RemoveAt(i);
                        }
                    }
                }
            }
        }
    }
    public class ItemElement : ElementBase
    {
        private RoomElement _room;

        public ItemElement(IModule[] options, Vector2Int position, RoomElement room)
        {
            _options = new List<IModule>(options);
            _position = position;
            _room = room;

            if (_position.x == 0 || _position.x == DungeonCreator.instance.GetRoomSize.x - 1 ||
                _position.y == 0 || _position.y == DungeonCreator.instance.GetRoomSize.y - 1)
            {
                _isEdge = true;
            }
            else _isEdge = false;
        }

        public override void Collapse()
        {
            RemoveOptionsFromPosition();

            int rng = Random.Range(0, _options.Count);
            _selectedModule = _options[rng];
        }

        public override void RemoveOptionsFromPosition()
        {
            DungeonCreator dc = DungeonCreator.instance;

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                ItemModule option = _options[i] as ItemModule;
                char locType = option.GetItemLocationType();
                char subType = option.GetItemSubType();

                switch (_room.GetRoomByteMap[_position.x, _position.y])
                {
                    case 0:
                        if (locType != 'F' && subType != 'N') // remove non floor type items
                            _options.RemoveAt(i);
                        break;
                    case 1:
                        if (locType != 'W' && subType != 'N') // remove non wall type items
                            _options.RemoveAt(i);
                        break;
                    case 2:// if tile is pit
                    case 3:// or tile is true path between exits
                    case 4:// or tile is dungeon entrance/exit, remove any items that aren't "None" type
                        if (subType != 'N')
                            _options.RemoveAt(i);
                        //Debug.Log("3 found in bytematp");
                        break;
                }
            }
        }
    }
}