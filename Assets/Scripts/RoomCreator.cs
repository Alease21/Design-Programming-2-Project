using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace WFC
{
    public class RoomCreator : MonoBehaviour
    {
        [SerializeField] private ItemCreator _itemCreator;

        [SerializeField] private TileSet _floorTileSet;
        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private Vector2Int _roomSize;
        private Vector2Int _numRoomsCompleted; // used for tracking room tile generation (done/max)

        public Action TileGenereationDone;

        public Vector2Int GetRoomSize { get { return _roomSize; } }

        private void Start()
        {
            _itemCreator = GetComponent<ItemCreator>();
        }
        public void GenerateRooms(Element[,] grid)
        {
            GameObject boundaryEmpty = new GameObject("DungeonBoundaries"); //parent to grid
            boundaryEmpty.transform.parent = _tileMap.transform.parent;
            boundaryEmpty.transform.position = Vector3.zero;

            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x,y] != null && grid[x, y].isTruePath)
                    {
                        StartCoroutine(CreateRoom(grid[x, y]));
                    }
                }
            }
        }
        public void ClearTiles()
        {
            _tileMap.ClearAllTiles();
        }
        public IEnumerator CreateRoom(Element room)
        {
            _numRoomsCompleted.y++;

            TileElement[,] tileGrid = new TileElement[_roomSize.x, _roomSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();
            Vector2Int[] posInfo = new Vector2Int[3];

            int pitPlaceRNG = UnityEngine.Random.Range(0, 10);

            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int x = 0; x < _roomSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x,y);
                    Vector2Int adjustedPosition = position + room.GetPosition * _roomSize;

                    bool isFloor = true, 
                         isExitFloor = false, 
                         isPit = false;
                    string exits = room.GetSelectedModule.GetDirString();
                    Vector2Int roomExitLocation = new Vector2Int((int)(_roomSize.x * 0.5 - 1), (int)(_roomSize.y * 0.5 - 1));
                    
                    if (room.GetSelectedModule.roomType == RoomModule.RoomType.PitRoom)
                    {
                        if (pitPlaceRNG >= 5)
                        {
                            Vector2Int quarterRoomSize = new Vector2Int((int)(_roomSize.x * 0.25f), (int)(_roomSize.y * 0.25f));
                            
                            if (x >= quarterRoomSize.x && x < _roomSize.x - quarterRoomSize.x && 
                                y >= quarterRoomSize.y && y < _roomSize.y - quarterRoomSize.y)
                            {
                                isPit = true;
                            }
                        }
                        else
                        {                            
                            if ((x < roomExitLocation.x - 1 || x > roomExitLocation.x + 2) &&
                                (y < roomExitLocation.y - 1 || y > roomExitLocation.y + 2))
                            {
                                isPit = true;
                            }
                        }
                    }
                    else
                    {
                        isPit = false;
                    }

                    if (x == 0 || y == 0)
                    {
                        isFloor = false;

                        if (exits[3] == 'W' && y >= roomExitLocation.y && y <= roomExitLocation.y + 1 ||
                            exits[2] == 'S' && x >= roomExitLocation.x && x <= roomExitLocation.x + 1)
                        {
                            isFloor = true;
                            isExitFloor = true;
                        }
                    }
                    else if (x == _roomSize.x - 1 || y == _roomSize.y - 1)
                    {
                        isFloor = false;
                        if (exits[1] == 'E' && y >= roomExitLocation.y && y <= roomExitLocation.y + 1 ||
                            exits[0] == 'N' && x >= roomExitLocation.x && x <= roomExitLocation.x + 1)
                        {
                            isFloor = true;
                            isExitFloor = true;
                        }
                    }

                    posInfo = new Vector2Int[] { position, adjustedPosition, _roomSize };
                    tileGrid[x, y] = new TileElement(_floorTileSet.tileModules, posInfo, isFloor, isPit);

                    if (isFloor)
                    {
                        tileGrid[x, y].RemoveOptions(TileModule.TileType.Wall);
                        //if (!isExitFloor) 
                            //_itemCreator.GenerateItems(posInfo);
                    }
                    if (isExitFloor || !isPit)
                        tileGrid[x,y].RemoveOptions(TileModule.TileType.Pit);
                    if (isPit)
                        tileGrid[x, y].RemoveOptions(TileModule.TileType.Floor);

                    unreachedPositions.Add(position);
                }
            }
            _itemCreator.AddTileGrid(tileGrid);

            int rng = UnityEngine.Random.Range(0, unreachedPositions.Count);

            CollapseElement(tileGrid[unreachedPositions[rng].x, unreachedPositions[rng].y], tileGrid);
            unreachedPositions.RemoveAt(rng);

            while (unreachedPositions.Count > 0)
            {
                TileElement curElement;
                List<TileElement> lowEntropyElements = new List<TileElement>();
                int lowestEntropy = int.MaxValue;

                for (int i = 0; i < unreachedPositions.Count; i++)
                {
                    curElement = tileGrid[unreachedPositions[i].x, unreachedPositions[i].y];
                    if (curElement.GetEntropy < lowestEntropy)
                    {
                        lowestEntropy = curElement.GetEntropy;
                        lowEntropyElements.Clear();
                        lowEntropyElements.Add(curElement);
                    }
                    else if (curElement.GetEntropy == lowestEntropy)
                    {
                        lowEntropyElements.Add(curElement);
                    }
                }

                rng = UnityEngine.Random.Range(0, lowEntropyElements.Count);
                curElement = lowEntropyElements[rng];

                CollapseElement(curElement, tileGrid);
                unreachedPositions.Remove(curElement.GetAdjustedPosition - room.GetPosition * _roomSize);
                yield return null;
            }

            _numRoomsCompleted.x++;
            if (_numRoomsCompleted.x == _numRoomsCompleted.y) 
                TileGenereationDone?.Invoke();
        }
        private void CollapseElement(TileElement curElement, TileElement[,] grid)
        {
            curElement.Collapse(_tileMap);
            
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) // setting up for neighbors, but not diags (1,1)
                    {
                        continue;
                    }

                    int curX = curElement.GetPosition.x + x;
                    int curY = curElement.GetPosition.y + y;

                    if ((curX < 0 || curY < 0) || (curX > grid.GetLength(0) - 1 || curY > grid.GetLength(1) - 1))
                    {
                        continue;
                    }

                    TileElement curNeighbour = grid[curX, curY];
                    
                    if (curElement.GetIsFloor)
                    {/*
                        if (x > 0)
                        {
                            curNeighbour.RemoveOptions(curElement.GetSelectedModule.east);
                        }
                        else if (x < 0)
                        {
                            curNeighbour.RemoveOptions(curElement.GetSelectedModule.west);
                        }
                        else if (y > 0)
                        {
                            curNeighbour.RemoveOptions(curElement.GetSelectedModule.north);
                        }
                        else if (y < 0)
                        {
                            curNeighbour.RemoveOptions(curElement.GetSelectedModule.south);
                        }
                        */
                    }
                }
            }
        }
    }

    public class TileElement
    {
        private List<TileModule> _options;
        private Vector2Int _position;
        private TileModule _selectedModule;
        private bool _isFloor = false;
        private bool _isPit = false;
        private Vector2Int _roomSize;
        private Vector2Int _adjustedPosition;

        public Vector2Int GetPosition { get { return _position; } }
        public Vector2Int GetAdjustedPosition { get { return _adjustedPosition; } }
        public Vector2Int GetRoomSize { get { return _roomSize; } }
        public TileModule GetSelectedModule { get { return _selectedModule; } }
        public int GetEntropy { get { return _options.Count; } }
        public bool GetIsFloor { get { return _isFloor; } }
        public bool GetIsPit { get { return _isPit; } }

        public TileElement(TileModule[] options, Vector2Int[] tileAndRoomPos,bool isFloor, bool isPit)
        {
            _options = new List<TileModule>(options);
            _position = tileAndRoomPos[0];
            _isFloor = isFloor;
            _isPit = isPit;
            _roomSize = tileAndRoomPos[2];
            _adjustedPosition = tileAndRoomPos[1];
        }

        public void RemoveOptions(TileModule[] illegalNeighbors)
        {
            List<TileModule> temp = new List<TileModule>(illegalNeighbors);
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (temp.Contains(_options[i]))
                {
                    _options.RemoveAt(i);
                }
            }
        }
        public void RemoveOptions(TileModule.TileType illegaltype)
        {
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (_options[i].tileType == illegaltype)
                {
                    _options.RemoveAt(i);
                }
            }
        }
        public void RemoveOptions(TileModule.TileType targetType, char legalChar)
        {
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (_options[i].tileType != targetType || 
                    _options[i].GetTileSubType() != legalChar)
                {
                    _options.RemoveAt(i);
                }
            }
        }

        public void Collapse(Tilemap tilemap)
        {
            if (!_isFloor)
            {
                RemoveWallOptions();
            }
            int rng = UnityEngine.Random.Range(0, _options.Count);
            //Debug.Log($"options count: {_options.Count}");

            // Weighted rng for different base floors
            if (_options[rng].tileType == TileModule.TileType.Floor)
            {
                int floorWeighRNG = UnityEngine.Random.Range(1, 100);

                switch (floorWeighRNG)
                {
                    case > 95:
                        RemoveOptions( TileModule.TileType.Floor, '5');
                        break;
                    case > 90:
                        RemoveOptions(TileModule.TileType.Floor, '3');
                        break;
                    case > 85:
                        RemoveOptions(TileModule.TileType.Floor, '2');
                        break;
                    case > 80:
                        RemoveOptions(TileModule.TileType.Floor, '1');
                        break;
                    default:
                        RemoveOptions(TileModule.TileType.Floor, '0');
                        break;
                }
                _selectedModule = _options[0];
            }
            else
            {
                _selectedModule = _options[rng];
            }

            tilemap.SetTile((Vector3Int)_adjustedPosition, _selectedModule.tileBase);
            if (!_isFloor)
            {
                GameObject tileBounds = new ($"({_adjustedPosition.x},{_adjustedPosition.y})");
                tileBounds.transform.parent = tilemap.transform.parent.Find("DungeonBoundaries");
                tileBounds.transform.position = new Vector3(_adjustedPosition.x + 0.5f, _adjustedPosition.y + 0.5f, 0);
                BoxCollider2D bc = tileBounds.AddComponent<BoxCollider2D>();
                bc.size = Vector2.one;
            }
        }
        public void RemoveWallOptions()
        {
            char[] dirChars = { 'N', 'E', 'S', 'W' };
            if (_position.x == 0)
            {
                if (_position.y == 0)
                {
                    dirChars[2] = '-';
                    dirChars[3] = '-';
                }
                else if (_position.y == _roomSize.y - 1)
                {
                    dirChars[0] = '-';
                    dirChars[3] = '-';
                }
                else
                {
                    dirChars[1] = '-';
                    dirChars[3] = '-';
                }
            }
            else if (_position.x == _roomSize.x - 1)
            {
                if (_position.y == 0)
                {
                    dirChars[1] = '-';
                    dirChars[2] = '-';
                }
                else if (_position.y == _roomSize.y - 1)
                {
                    dirChars[0] = '-';
                    dirChars[1] = '-';
                }
                else
                {
                    dirChars[1] = '-';
                    dirChars[3] = '-';
                }
            }
            else
            {
                dirChars[0] = '-';
                dirChars[2] = '-';
            }
            string edgeDir = new string(dirChars);

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                string curModuleDirections = "Z";
                if (_options[i].tileType == TileModule.TileType.Wall) 
                    curModuleDirections = _options[i].GetWallDirections();

                if (_options[i].tileType != TileModule.TileType.Wall || !curModuleDirections.Contains(edgeDir))
                {
                    _options.RemoveAt(i);
                }
            }
        }
    }
}