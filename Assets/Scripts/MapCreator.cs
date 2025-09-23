using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace WFC
{
    public class MapCreator : MonoBehaviour
    {
        [Tooltip("Map size in rooms")]
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private RoomModuleSet _roomSet;
        private bool _exitMade = false;
        private Element _startRoom, _exitRoom;
        private Element[,] grid;
        int numPathsOpen = 0;

        private void Start()
        {
            Generate();
        }

        public void Generate()
        {
            StartCoroutine(CreateWorld());
        }
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                RestartGeneration();
            }
        }
        public void RestartGeneration()
        {
            StopAllCoroutines();

            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            _exitMade = false;

            Generate();
        }
        private IEnumerator CreateWorld()
        {
            grid = new Element[_mapSize.x, _mapSize.y];
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

            _startRoom = startRoom;
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

                float distFromStart = (float)(curElement.GetPosition - startRoom.GetPosition).magnitude;

                if ((float)unreachedPositions.Count / (float)(grid.GetLength(0) * grid.GetLength(1)) < 0.25f && 
                    distFromStart > ((grid.GetLength(0) + grid.GetLength(1)) * 0.5 * 0.75)) // if distance from start is > (3/4) * avg between grid size components, 
                {
                    if (curElement.GetEdgeBool && !_exitMade)
                    {
                        _allowExit = true;
                        _exitMade = true;
                        _exitRoom = curElement;
                    }
                }

                CollapseElement(curElement, grid, _allowExit);
                unreachedPositions.Remove(curElement.GetPosition);

                yield return null;
            }
            StartCoroutine(SearchPathCoro());
        } 
        private void CollapseElement(Element curElement, Element[,] grid, bool allowEnterExit)
        {
            curElement.Collapse(allowEnterExit);

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

        public IEnumerator SearchPathCoro()
        {
            SearchTruePath(_startRoom);
            yield return new WaitUntil(() => numPathsOpen == 0);
            //yield return new WaitForSeconds(1);
            if (!grid[_exitRoom.GetPosition.x, _exitRoom.GetPosition.y].isTruePath)
            {
                RestartGeneration();
            }
            else
            {
                for (int x = 0; x < grid.GetLength(0) ; x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        if (!grid[x, y].isTruePath)
                        {
                            grid[x, y] = null;
                        }
                        else
                        {
                            GameObject newRoomGO = GameObject.Instantiate(Resources.Load<GameObject>("RoomEmpty"), (Vector3Int)(grid[x, y].GetPosition * 10), Quaternion.identity, transform);
                            SpriteRenderer roomRenderer = newRoomGO.GetComponentInChildren<SpriteRenderer>();
                            roomRenderer.sprite = grid[x,y].GetSelectedModule.roomSprite;

                            if (x == _startRoom.GetPosition.x && y == _startRoom.GetPosition.y)
                            {
                                roomRenderer.color = Color.green;
                            }
                            else if (x == _exitRoom.GetPosition.x && y == _exitRoom.GetPosition.y)
                            {
                                roomRenderer.color = Color.red;
                            }
                        }
                    }
                }
            }
        }
        public void SearchTruePath(Element currElement, char trueNeighborDir = 'Z')
        {
            if (currElement.isTruePath)
                return;
            currElement.isTruePath = true;

            Dictionary<Element,char> trueNeighbours = new();
            string dirs = currElement.GetSelectedModule.GetDirString();

            foreach (char dir in dirs)
            {
                Vector2Int neighbourPos;
                if (dir == 'N' && trueNeighborDir != 'N')
                {
                    neighbourPos = new Vector2Int(currElement.GetPosition.x, currElement.GetPosition.y + 1);
                    if (CheckIfInBounds(neighbourPos))
                        trueNeighbours.Add(grid[neighbourPos.x, neighbourPos.y], 'S');
                }
                else if (dir == 'E' && trueNeighborDir != 'E')
                {
                    neighbourPos = new Vector2Int(currElement.GetPosition.x + 1, currElement.GetPosition.y);
                    if (CheckIfInBounds(neighbourPos))
                        trueNeighbours.Add(grid[neighbourPos.x, neighbourPos.y], 'W');
                }
                else if (dir == 'S' && trueNeighborDir != 'S')
                {
                    neighbourPos = new Vector2Int(currElement.GetPosition.x, currElement.GetPosition.y - 1);
                    if (CheckIfInBounds(neighbourPos))
                        trueNeighbours.Add(grid[neighbourPos.x, neighbourPos.y], 'N');
                }
                else if (dir == 'W' && trueNeighborDir != 'W')
                {
                    neighbourPos = new Vector2Int(currElement.GetPosition.x - 1, currElement.GetPosition.y);
                    if (CheckIfInBounds(neighbourPos))
                        trueNeighbours.Add(grid[neighbourPos.x, neighbourPos.y], 'E');
                }
            }
            numPathsOpen += trueNeighbours.Count;

            foreach (KeyValuePair<Element,char> kvp in trueNeighbours)
            {
                SearchTruePath(kvp.Key, kvp.Value);
                numPathsOpen--;
            }
        }

        public bool CheckIfInBounds(Vector2Int pos)
        {
            if (pos.x >= _mapSize.x || pos.x < 0 || pos.y >= _mapSize.y || pos.y < 0)
            {
                return false;
            }
            return true;
        }
    }
    public class Element
    {
        private List<RoomModule> _options;
        private Vector2Int _position;
        private RoomModule _selectedModule;
        private bool _isEdge;
        private Vector2Int _mapSize;

        public bool isTruePath = false; //make better?

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
        public void Collapse(bool allowEnterExit)
        {
            if (_isEdge)
            {
                RemoveEdgeOptions(allowEnterExit);
            }
            int rng = Random.Range(0, _options.Count);
            //Debug.Log("Edge:"+_isEdge+"  Options count: " + _options.Count+$" at {_position.x}, {_position.y}");
            _selectedModule = _options[rng];
        }
        public void RemoveEdgeOptions(bool allowEnterExit)
        {
            char edgeNS = 'Z'; // Z used just because it is not found in directions
            char edgeEW = 'Z'; // and unsure if using (char != "") is a good method

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
                string curModuleDirections = _options[i].GetDirString();
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