using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace WFC
{
    public class MapCreator : MonoBehaviour
    {
        //Singleton setup
        public static MapCreator instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(this.gameObject);
            }

            _roomSet.SetNeighbours();
        }
        //

        [Tooltip("Map size in rooms")]
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private RoomModuleSet _roomSet;

        private bool _exitMade = false;
        private RoomElement _startRoom, 
                        _exitRoom;
        private RoomElement[,] grid;
        private int numPathsOpen = 0 , 
                    numDungeonTiles = 0;

        Stopwatch _stopWatch = new Stopwatch();

        private void Start()
        {
            ItemCreator.instance.ItemGenerationDone += GenerationTimer;
            Generate();
        }
        private void Update()
        {
            // Quick input to regenerate world, remove later
            if (Input.GetKeyDown(KeyCode.G))
            {
                RestartGeneration();
            }
        }

        public void Generate()
        {
            RoomCreator rc = RoomCreator.instance;
            transform.position = new Vector3(rc.GetRoomSize.x * 0.5f, rc.GetRoomSize.y * 0.5f, 0f);

            StartCoroutine(CreateWorld());
        }

        // Displays elapsed time during dungeon generation
        public void GenerationTimer()
        {
            _stopWatch.Stop();

            UnityEngine.Debug.Log($"Genereation Time: {_stopWatch.Elapsed}");
            _stopWatch.Reset();
        }
        
        // Delete current dungeon rooms, tile, items and restart generation
        public void RestartGeneration()
        {
            StopAllCoroutines();

            for (int i = 0; i < transform.childCount; i++)
            {
                Destroy(transform.GetChild(i).gameObject);
            }
            RoomCreator.instance.ClearTiles();
            ItemCreator.instance.ClearTiles();

            _exitMade = false;
            numDungeonTiles = 0;
            numPathsOpen = 0;

            Generate();
        }

        #region World Creation

        // Create and populate room of grid elements then loop through each element and collapse it.
        private IEnumerator CreateWorld()
        {
            _stopWatch.Start();

            grid = new RoomElement[_mapSize.x, _mapSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();
            int rng;

            for (int y = 0; y < _mapSize.y; y++)
            {
                for (int x = 0; x < _mapSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    grid[x, y] = new RoomElement(_roomSet.roomModules, position, _mapSize);
                    unreachedPositions.Add(position);
                }
            }

            // Guarantee startroom on the bottom edge
            do
            {
                rng = UnityEngine.Random.Range(0, _mapSize.x);//always start from bottom row
                _startRoom = grid[unreachedPositions[rng].x, unreachedPositions[rng].y];
            }
            while (!_startRoom.GetEdgeBool); 

            CollapseElement(_startRoom, grid, true);
            unreachedPositions.RemoveAt(rng);

            // Find lowest entropy element(s)
            while (unreachedPositions.Count > 0)
            {
                RoomElement curElement;
                List<RoomElement> lowEntropyElements = new List<RoomElement>();
                int lowestEntropy = int.MaxValue;
                bool _allowExit = false;

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

                rng = UnityEngine.Random.Range(0, lowEntropyElements.Count);
                curElement = lowEntropyElements[rng];

                float distFromStart = (float)(curElement.GetPosition - _startRoom.GetPosition).magnitude;

                // Define minimum range for exit spawn
                if ((float)unreachedPositions.Count / (float)(grid.GetLength(0) * grid.GetLength(1)) < 0.25f &&
                    distFromStart > ((grid.GetLength(0) + grid.GetLength(1)) * 0.5 * 0.75)) 
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

        // Determine specific room from list of available options and remove illegal options for neighours
        private void CollapseElement(RoomElement curElement, RoomElement[,] grid, bool allowEnterExit)
        {
            curElement.Collapse(allowEnterExit);

            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) // setting up for neighbours, but not diags (1,1)
                        continue;

                    int curX = curElement.GetPosition.x + x;
                    int curY = curElement.GetPosition.y + y;

                    if ((curX < 0 || curY < 0) || (curX > grid.GetLength(0) - 1 || curY > grid.GetLength(1) - 1))
                        continue;

                    RoomElement curNeighbour = grid[curX, curY];

                    if (x > 0)
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.east);
                    else if (x < 0)
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.west);
                    else if (y > 0)
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.north);
                    else if (y < 0)
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.south);
                }
            }
        }
        #endregion
        #region True Path Recurive Method
        // Start recursive path search algroithm and wait for result. If no true path found to exit, restart generation,
        // else create placeholder room tiles
        public IEnumerator SearchPathCoro()
        {
            SearchTruePath(_startRoom);
            yield return new WaitUntil(() => numPathsOpen == 0);

            if (_exitRoom == null) { UnityEngine.Debug.Log("Exit room null"); }

            if (numDungeonTiles > (0.75f * _mapSize.x * _mapSize.y) || !_exitRoom.isTruePath)
            {
                RestartGeneration();
            }
            else
            {
                for (int x = 0; x < grid.GetLength(0); x++)
                {
                    for (int y = 0; y < grid.GetLength(1); y++)
                    {
                        if (!grid[x, y].isTruePath)
                        {
                            grid[x, y] = null;
                        }
                        else
                        {
                            RoomCreator rc = RoomCreator.instance;
                            grid[x, y].GetSelectedModule.roomType = (RoomModule.RoomType)UnityEngine.Random.Range((int)0, 2);
                            GameObject newRoomGO = GameObject.Instantiate(Resources.Load<GameObject>("RoomEmpty"), transform);
                            SpriteRenderer roomRenderer = newRoomGO.GetComponentInChildren<SpriteRenderer>();
                            Transform newTileTrans = newRoomGO.transform.GetChild(0).transform;

                            newRoomGO.transform.localPosition = new Vector3Int(grid[x, y].GetPosition.x * rc.GetRoomSize.x, 
                                grid[x, y].GetPosition.y * rc.GetRoomSize.y, (int)newRoomGO.transform.localPosition.z);
                            roomRenderer.sprite = grid[x, y].GetSelectedModule.roomSprite;
                            newTileTrans.localScale = new Vector3(newTileTrans.localScale.x * rc.GetRoomSize.x,
                                newTileTrans.localScale.y * rc.GetRoomSize.y, newTileTrans.localScale.z);

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
                RoomCreator.instance.GenerateRooms(grid);
            }
        }

        // Recursive method to search for true path starting from start room. 
        public void SearchTruePath(RoomElement currElement, char trueNeighborDir = 'Z')
        {
            if (currElement.isTruePath) return;
            currElement.isTruePath = true;
            numDungeonTiles++;

            Dictionary<RoomElement, char> trueNeighbours = new();
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

            foreach (KeyValuePair<RoomElement, char> kvp in trueNeighbours)
            {
                SearchTruePath(kvp.Key, kvp.Value);
                numPathsOpen--;
            }
        }

        public bool CheckIfInBounds(Vector2Int pos)
        {
            if (pos.x >= _mapSize.x || pos.x < 0 || pos.y >= _mapSize.y || pos.y < 0)
                return false;
            return true;
        }
        #endregion
    }
    #region Element
    public class RoomElement
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

        public RoomElement(RoomModule[] options, Vector2Int position, Vector2Int mapSize)
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
                    _options.RemoveAt(i);
            }
        }
        public void Collapse(bool allowEnterExit)
        {
            if (_isEdge)
                RemoveEdgeOptions(allowEnterExit);
            int rng = UnityEngine.Random.Range(0, _options.Count);
            _selectedModule = _options[rng];
        }
        public void RemoveEdgeOptions(bool allowEnterExit)
        {
            char edgeNS = 'Z';
            char edgeEW = 'Z';

            if (_position.x == 0)
                edgeEW = 'W';
            else if (_position.x == _mapSize.x - 1)
                edgeEW = 'E';
            if (_position.y == 0)
                edgeNS = 'S';
            else if (_position.y == _mapSize.y - 1)
                edgeNS = 'N';

            for (int i = _options.Count - 1; i >= 0; i--)
            {
                string curModuleDirections = _options[i].GetDirString();
                int dirCounter = 0;

                foreach (char dir in curModuleDirections)
                {
                    if (dir != '-')
                        dirCounter++;
                }

                if (allowEnterExit)
                {
                    if ((edgeNS != 'Z' && !curModuleDirections.Contains(edgeNS)) || (edgeEW != 'Z' && 
                        !curModuleDirections.Contains(edgeEW)) || dirCounter == 1)
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
    #endregion
}