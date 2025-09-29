using UnityEngine;
using System.Collections.Generic;

namespace WFC
{
    public class RoomElement : ElementBase
    {
        private bool _isTruePath = false;

        public bool IsTruePath { get { return _isTruePath; } set { _isTruePath = value; } }

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
                    !dc.IsExitMade && distFromStart > 0.65f * dc.GetMapSize.magnitude)
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

    public class TileElement : ElementBase
    {
        public TileElement(IModule[] options, Vector2Int position)
        {
            _options = new List<IModule>(options);
            _position = position;

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
            Debug.Log($"Option Count: {_options.Count} @ ({_position.x + DungeonCreator.instance.CurrentRoom.GetPosition.x * DungeonCreator.instance.GetRoomSize.x}," +
                $"{_position.y + DungeonCreator.instance.CurrentRoom.GetPosition.y * DungeonCreator.instance.GetRoomSize.y})");
            _selectedModule = _options[rng];
        }

        public override void RemoveOptionsFromPosition()
        {
            DungeonCreator dc = DungeonCreator.instance;

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                TileModule option = _options[i] as TileModule;
                RoomModule curRM = dc.CurrentRoom.GetSelectedModule as RoomModule;
                string roomExitDirs = curRM.GetDirString();

                if (!_isEdge)
                {
                    if (option.GetTileType == TileModule.TileType.Wall)
                        _options.RemoveAt(i);
                }
                else
                {
                    if (option.GetTileType != TileModule.TileType.Wall)
                    {
                        _options.RemoveAt(i);
                    }

                        /*
                        else
                        {
                            if (_position.x == 0 )
                            {
                                if (_position.y == 0)
                                {
                                    if (curRM.GetDirString()[2] == 'S' || curRM.GetDirString()[3] == 'W')
                                        _options.RemoveAt(i);
                                }
                                else if (_position.y == dc.GetRoomSize.y - 1)
                                {
                                    if (curRM.GetDirString()[0] == 'N' || curRM.GetDirString()[3] == 'W')
                                        _options.RemoveAt(i);
                                }
                            }
                            else if (_position.x == dc.GetRoomSize.x - 1)
                            {
                                if (_position.y == 0)
                                {
                                    if (curRM.GetDirString()[2] == 'S' || curRM.GetDirString()[1] == 'E')
                                        _options.RemoveAt(i);
                                }
                                else if (_position.y == dc.GetRoomSize.y - 1)
                                {
                                    if (curRM.GetDirString()[0] == 'N' || curRM.GetDirString()[3] == 'W')
                                        _options.RemoveAt(i);
                                }
                            }
                        }
                        */
                    }
                }
        }
    }

    public class ItemElement : ElementBase
    {
        public ItemElement(IModule[] options, Vector2Int position, Vector2Int roomSize)
        {
            _options = new List<IModule>(options);
            _position = position;

            if (_position.x == 0 || _position.x == DungeonCreator.instance.GetRoomSize.x - 1 ||
                _position.y == 0 || _position.y == DungeonCreator.instance.GetRoomSize.y - 1)
            {
                _isEdge = true;
            }
            else _isEdge = false;
        }

        public override void Collapse()
        {
            //
        }
    }
}