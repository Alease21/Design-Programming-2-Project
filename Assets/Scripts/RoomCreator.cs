using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    public class RoomCreator : MonoBehaviour
    {
        [SerializeField] private TileSet _floorTileSet, _wallTileSet;
        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private Vector2Int _roomSize;

        public Vector2Int GetRoomSize { get { return _roomSize; } }

        public void GenerateRooms(Element[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    if (grid[x,y] != null && grid[x,y].isTruePath)
                        StartCoroutine(CreateRoom(grid[x, y]));
                }
            }
        }

        public IEnumerator CreateRoom(Element room)
        {
            TileElement[,] tileGrid = new TileElement[_roomSize.x, _roomSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();

            for (int y = 0; y < _roomSize.y; y++)
            {
                for (int x = 0; x < _roomSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x,y);
                    Vector2Int adjustedPosition = position + room.GetPosition * _roomSize;
                    bool isFloor = true;

                    if (x == 0)
                    {
                        string exits = room.GetSelectedModule.GetDirString();

                        isFloor = false;
                        if (exits[3] == 'W' && y >= (int)(_roomSize.y * 0.5f - 1) && y <= (int)(_roomSize.y * 0.5f - 1))
                        {
                            isFloor = true;
                        }
                    }
                    else if (x == _roomSize.x - 1)
                    {
                        string exits = room.GetSelectedModule.GetDirString();

                        isFloor = false;
                        if (exits[1] == 'E' && y >= (int)(_roomSize.y * 0.5f - 1) && y <= (int)(_roomSize.y * 0.5f - 1))
                        {
                            isFloor = true;
                        }
                    }
                    else if (y == 0)
                    {
                        string exits = room.GetSelectedModule.GetDirString();

                        isFloor = false;
                        if (exits[2] == 'S' && x >= (int)(_roomSize.x * 0.5f - 1) && x <= (int)(_roomSize.x * 0.5f - 1))
                        {
                            isFloor = true;
                        }
                    }
                    else if (y == _roomSize.y - 1)
                    {
                        string exits = room.GetSelectedModule.GetDirString();

                        isFloor = false;
                        if (exits[0] == 'N' && x >= (int)(_roomSize.x * 0.5f - 1) && x <= (int)(_roomSize.x * 0.5f - 1))
                        {
                            isFloor = true;
                        }
                    }

                    if (isFloor)
                    {
                        tileGrid[x, y] = new TileElement(_floorTileSet.tileModules, adjustedPosition, isFloor);
                    }
                    else
                    {
                        tileGrid[x, y] = new TileElement(_wallTileSet.tileModules, adjustedPosition, isFloor);
                    }
                    unreachedPositions.Add(position);
                }
            }
            int rng = Random.Range(0, unreachedPositions.Count);

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

                rng = Random.Range(0, lowEntropyElements.Count);
                curElement = lowEntropyElements[rng];

                CollapseElement(curElement, tileGrid);
                unreachedPositions.Remove(curElement.GetPosition - room.GetPosition * _roomSize);
                yield return null;
            }
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

                    if (x > 0 && (curNeighbour.GetIsFloor != curElement.GetIsFloor))
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.east);
                    }
                    else if (x < 0 && (curNeighbour.GetIsFloor != curElement.GetIsFloor))
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.west);
                    }
                    else if (y > 0 && (curNeighbour.GetIsFloor != curElement.GetIsFloor))
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.north);
                    }
                    else if (y < 0 && (curNeighbour.GetIsFloor != curElement.GetIsFloor))
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.south);
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

        public Vector2Int GetPosition { get { return _position; } }
        public TileModule GetSelectedModule { get { return _selectedModule; } }
        public int GetEntropy { get { return _options.Count; } }
        public bool GetIsFloor { get { return _isFloor; } }

        public TileElement(List<TileModule> options, Vector2Int position, bool isFloor)
        {
            _options = options;
            _position = position;
            _isFloor = isFloor;
        }
        public TileElement(TileModule[] options, Vector2Int position, bool isFloor)
        {
            _options = new List<TileModule>(options);
            _position = position;
            _isFloor = isFloor;
        }

        public void RemoveOptions(TileModule[] legalNeighbors)
        {
            List<TileModule> temp = new List<TileModule>(legalNeighbors);
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (temp.Contains(_options[i]) == false)
                {
                    _options.RemoveAt(i);
                }
            }
        }

        public void Collapse(Tilemap tilemap)
        {
            int rng = Random.Range(0, _options.Count);
            //Debug.Log($"options count: {_options.Count}");
            _selectedModule = _options[rng];

            tilemap.SetTile((Vector3Int)_position, _selectedModule.tileBase);
        }
    }
}