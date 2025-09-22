using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    public class MapCreator : MonoBehaviour
    {
        [Tooltip("Map size in rooms")]
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private RoomModuleSet _roomSet;
        private bool _exitMade = false;
        //private Element _startRoom, _exitRoom;

        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            StartCoroutine(CreateWorld());
        }

        private IEnumerator CreateWorld()
        {
            Element[,] grid = new Element[_mapSize.x, _mapSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();

            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    grid[x, y] = new Element(_roomSet.roomModules, position, _mapSize);
                    unreachedPositions.Add(position);
                }
            }

            int rng;
            Element startRoom;
            do
            {
                rng = Random.Range(0, _mapSize.x);//always start from bottom row
                startRoom = grid[unreachedPositions[rng].x, unreachedPositions[rng].y];
            }
            while (!startRoom.GetEdgeBool);

            //_startRoom = startRoom;
            CollapseElement(startRoom, grid, true);

            unreachedPositions.RemoveAt(rng);

            while (unreachedPositions.Count > 0)
            {
                Element curElement;
                List<Element> lowEntropyElements = new List<Element>();
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

                bool _allowExit = false;
                rng = Random.Range(0, lowEntropyElements.Count);
                curElement = lowEntropyElements[rng];

                float distFromStart = (curElement.GetPosition - startRoom.GetPosition).magnitude * 10; //maybe fix to adjust with room size

                if ((float)unreachedPositions.Count / (grid.GetLength(0) * grid.GetLength(1)) < 0.75f &&
                    distFromStart > 80f) //maybe fix to adjust with room size
                {
                    if (curElement.GetEdgeBool && !_exitMade)
                    {
                        _allowExit = true;
                        _exitMade = true;
                        //_exitRoom = curElement;
                    }
                }

                CollapseElement(curElement, grid, _allowExit);
                unreachedPositions.Remove(curElement.GetPosition);

                yield return null;
            }
        } 
        private void CollapseElement(Element curElement, Element[,] grid, bool allowEnterExit)
        {
            curElement.Collapse(transform, allowEnterExit);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) // setting up for neighbours, but not diags (1,1)
                    {
                        continue;
                    }

                    int curX = curElement.GetPosition.x + x;
                    int curY = curElement.GetPosition.y + y;

                    if ((curX < 0 || curY < 0) || (curX > grid.GetLength(0) - 1 || curY > grid.GetLength(1) - 1))
                    {
                        continue;
                    }

                    Element curNeighbour = grid[curX, curY];

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
    public class Element
    {
        private List<RoomModule> _options;
        private Vector2Int _position;
        private RoomModule _selectedModule;
        private bool _isEdge;
        private Vector2Int _mapSize;

        public Vector2Int GetPosition { get { return _position; } }
        public RoomModule GetSelectedModule { get { return _selectedModule; } }
        public int GetEntropy { get { return _options.Count; } }
        public bool GetEdgeBool { get { return _isEdge; } }

        public Element(RoomModule[] options, Vector2Int position, Vector2Int mapSize)
        {
            _options = new List<RoomModule>(options);
            _position = position;
            _mapSize = mapSize;
            _isEdge = CheckForEdgePosition(mapSize);
        }
        public void RemoveOptions(RoomModule[] legalNeighbors)
        {
            List<RoomModule> temp = new List<RoomModule>(legalNeighbors);

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (temp.Contains(_options[i]) == false)
                {
                    _options.RemoveAt(i);
                }
            }
        }
        public void Collapse(Transform parent, bool allowEnterExit)
        {
            if (_isEdge)
            {
                RemoveEdgeOptions(allowEnterExit);
            }
            int rng = Random.Range(0, _options.Count);
            Debug.Log("Edge:"+_isEdge+"  Options count: " + _options.Count+$" at {_position.x}, {_position.y}");
            _selectedModule = _options[rng];

            GameObject newRoomGO = GameObject.Instantiate(Resources.Load<GameObject>("RoomEmpty"), (Vector3Int)(_position * 10), Quaternion.identity, parent);
            newRoomGO.GetComponentInChildren<SpriteRenderer>().sprite = _selectedModule.roomSprite;
        }
        public void RemoveEdgeOptions(bool allowEnterExit)
        {
            char edgeNS = 'Z';
            char edgeEW = 'Z';

            if (_position.x == 0)
            {
                edgeEW = 'W';
            }
            else if (_position.x == _mapSize.x -1)
            {
                edgeEW = 'E';
            }
            if (_position.y == 0)
            {
                edgeNS = 'S';
            }
            else if (_position.y == _mapSize.y - 1)
            {
                edgeNS = 'N';
            }

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                string curModuleDirections = _options[i].name.Substring(_options[i].name.Length - 4);
                int dirCounter = 0;
                foreach (char dir in curModuleDirections)
                {
                    if (dir != '-')
                    {
                        dirCounter++;
                    }
                }

                if (allowEnterExit)
                {
                    if ((edgeNS != 'Z' && !curModuleDirections.Contains(edgeNS)) || (edgeEW != 'Z' && !curModuleDirections.Contains(edgeEW)) 
                        || dirCounter == 1)
                    {
                        _options.RemoveAt(i);
                    }
                }
                else
                {
                    if ((edgeNS != 'Z' && curModuleDirections.Contains(edgeNS)) || (edgeEW != 'Z' && curModuleDirections.Contains(edgeEW)))
                    {
                        _options.RemoveAt(i);
                    }
                }
            }
        }
        public bool CheckForEdgePosition(Vector2Int mapSize)
        {
            if (_position.x == 0 || _position.x == mapSize.x - 1 ||
                _position.y == 0 || _position.y == mapSize.y - 1)
            {
                return true;
            }
            return false;
        }
    }
}