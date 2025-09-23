using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    public class RoomCreator : MonoBehaviour
    {
        [SerializeField] private TileSet _tileSet;
        [SerializeField] private Tilemap _tileMap;
        [SerializeField] private Vector2Int _roomSize;

        public void GenerateRooms(TileElement[,] grid)
        {
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(1); y++)
                {
                    StartCoroutine(CreateRoom(grid[x, y], _roomSize));
                }
            }
        }

        public IEnumerator CreateRoom(TileElement element, Vector2Int roomSize)
        {
            TileElement[,] grid = new TileElement[roomSize.x, roomSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();

            for (int y = 0; y < roomSize.y; y++)
            {
                for (int x = 0; x < roomSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    grid[x, y] = new TileElement(_tileSet.tileModules, new Vector2Int(x, y));
                    unreachedPositions.Add(position);
                }
            }
            int rng = Random.Range(0, unreachedPositions.Count);

            CollapseElement(grid[unreachedPositions[rng].x, unreachedPositions[rng].y], grid);
            unreachedPositions.RemoveAt(rng);

            while (unreachedPositions.Count > 0)
            {
                TileElement curElement;
                List<TileElement> lowEntropyElements = new List<TileElement>();
                int lowestEntropy = int.MaxValue;

                for (int i = 0; i < unreachedPositions.Count; i++)
                {
                    curElement = grid[unreachedPositions[i].x, unreachedPositions[i].y];
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

                CollapseElement(curElement, grid);
                unreachedPositions.Remove(curElement.GetPosition);

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
                }
            }
        }
    }

    public class TileElement
    {
        private List<TileModule> _options;
        private Vector2Int _position;
        private TileModule _selectedModule;

        public Vector2Int GetPosition { get { return _position; } }
        public TileModule GetSelectedModule { get { return _selectedModule; } }
        public int GetEntropy { get { return _options.Count; } }

        public TileElement(List<TileModule> options, Vector2Int position)
        {
            _options = options;
            _position = position;
        }
        public TileElement(TileModule[] options, Vector2Int position)
        {
            _options = new List<TileModule>(options);
            _position = position;
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
            _selectedModule = _options[rng];

            tilemap.SetTile((Vector3Int)_position, _selectedModule.tileBase);
        }
    }
}