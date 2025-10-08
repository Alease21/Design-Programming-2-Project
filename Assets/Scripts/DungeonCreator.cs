using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static WFC.WaveFunctionCollapse;

namespace WFC
{
    public class DungeonCreator : MonoBehaviour
    {
        // Singleton setup
        public static DungeonCreator instance;
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
            //

            //Initial neighbor sets in case project didn't load with them correctly
            _roomSet.SetNeighbours();
            _tileSet.SetNeighbours();
            _itemSet.SetNeighbours();
        }

        [SerializeField] private RoomSet _roomSet;
        [SerializeField] private TileSet _tileSet;
        [SerializeField] private ItemSet _itemSet;
        private RoomElement[,] _roomGrid;

        [SerializeField] Vector2Int _mapSize;
        [SerializeField] Vector2Int _roomSize;
        [SerializeField] public Tilemap _environTileMap;
        [SerializeField] Tilemap _itemTileMap;

        private bool _isStartMade = false,
                     _isExitMade = false;
        private RoomElement _startRoom,
                            _exitRoom;
        private int numPathsOpen = 0,
                    numDungeonTiles = 0;

        private Stopwatch _stopWatch = new Stopwatch();

        public RoomElement[,] GetRoomGrid { get { return _roomGrid; } }
        public bool IsStartMade { get { return _isStartMade; } set { _isStartMade = value; } }
        public bool IsExitMade { get { return _isExitMade; } set { _isExitMade = value; } }
        public RoomElement StartRoom { get { return _startRoom; } set { _startRoom = value; } }
        public RoomElement ExitRoom { get { return _exitRoom; } set { _exitRoom = value; } }
        public Vector2Int GetMapSize { get { return _mapSize; } }
        public Vector2Int GetRoomSize { get { return _roomSize; } }

        private void Start()
        {
            GameObject boundaryEmpty = new GameObject("DungeonBoundaries");
            boundaryEmpty.transform.parent = _environTileMap.transform.parent;
            boundaryEmpty.transform.position = Vector3.zero;

            CollapseRooms();
        }
        private void Update()
        {
            // Quick input to regenerate world, remove later
            if (Input.GetKeyDown(KeyCode.G))
            {
                RestartGeneration();
            }
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
            _environTileMap.ClearAllTiles();
            _itemTileMap.ClearAllTiles();
            _isStartMade = false;
            _isExitMade = false;
            numDungeonTiles = 0;
            numPathsOpen = 0;

            CollapseRooms();
        }
        public void CollapseRooms()
        {
            _stopWatch.Start();
            _roomGrid = new RoomElement[_roomSize.x, _roomSize.y];

            _roomGrid = WFCGenerate(_roomSet.Modules, _mapSize) as RoomElement[,];
            StartCoroutine(SearchPathCoro());
        }

        public void CollapseItems(RoomElement room)
        {
            ItemElement[,] curItemGrid = WFCGenerate(_itemSet.Modules, _roomSize, room) as ItemElement[,];
            CreateItems(curItemGrid, room);
        }

        #region True Path Recursion
        // Start recursive path search algroithm and wait for result. If no true path found to exit, restart generation,
        // else create placeholder room tiles
        public IEnumerator SearchPathCoro()
        {
            SearchTruePath(_startRoom);
            yield return new WaitUntil(() => numPathsOpen == 0);

            if (_exitRoom == null) { UnityEngine.Debug.Log("Exit room null"); }

            if (numDungeonTiles > (0.75f * _mapSize.x * _mapSize.y) || !_exitRoom.IsTruePath)
            {
                RestartGeneration();
            }
            else
            {
                for (int x = 0; x < _roomGrid.GetLength(0); x++)
                {
                    for (int y = 0; y < _roomGrid.GetLength(1); y++)
                    {
                        if (!_roomGrid[x, y].IsTruePath)
                        {
                            _roomGrid[x, y] = null;
                        }
                        else
                        {
                            Vector2Int pos = new Vector2Int(x, y);
                            CreateRoom(_roomGrid[x, y]);
                            CollapseItems(_roomGrid[x, y]);

                            //Item WFC, item place, and add grid to list
                            yield return null;
                        }
                    }
                }
                GenerationTimer();
            }
        }

        // Recursive method to search for true path starting from start room. 
        public void SearchTruePath(RoomElement currElement, char trueNeighborDir = 'Z')
        {
            if (currElement.IsTruePath) return;
            currElement.IsTruePath = true;
            numDungeonTiles++;

            Dictionary<RoomElement, char> trueNeighbours = new();
            RoomModule curModule = currElement.GetSelectedModule as RoomModule;
            string dirs = curModule.GetDirString();

            foreach (char dir in dirs)
            {
                Vector2Int curPos = currElement.GetPosition;
                Vector2Int neighbourPos;
                if (dir == 'N' && trueNeighborDir != 'N')
                {
                    neighbourPos = curPos + Vector2Int.up;
                    if (CheckIfInBounds(neighbourPos) && _roomGrid[neighbourPos.x, neighbourPos.y] != null)
                        trueNeighbours.Add(_roomGrid[neighbourPos.x, neighbourPos.y], 'S');
                }
                else if (dir == 'E' && trueNeighborDir != 'E')
                {
                    neighbourPos = curPos + Vector2Int.right;
                    if (CheckIfInBounds(neighbourPos) && _roomGrid[neighbourPos.x, neighbourPos.y] != null)
                        trueNeighbours.Add(_roomGrid[neighbourPos.x, neighbourPos.y], 'W');
                }
                else if (dir == 'S' && trueNeighborDir != 'S')
                {
                    neighbourPos = curPos + Vector2Int.down;
                    if (CheckIfInBounds(neighbourPos) && _roomGrid[neighbourPos.x, neighbourPos.y] != null)
                        trueNeighbours.Add(_roomGrid[neighbourPos.x, neighbourPos.y], 'N');
                }
                else if (dir == 'W' && trueNeighborDir != 'W')
                {
                    neighbourPos = curPos + Vector2Int.left;
                    if (CheckIfInBounds(neighbourPos) && _roomGrid[neighbourPos.x, neighbourPos.y] != null)
                        trueNeighbours.Add(_roomGrid[neighbourPos.x, neighbourPos.y], 'E');
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

        public void CreateRoom(RoomElement room)
        {
            RoomModule roomModule = room.GetSelectedModule as RoomModule;
            roomModule.RoomType = (RoomModule.RoomTypes)Random.Range((int)0, 2);

            GameObject newRoomGO = GameObject.Instantiate(Resources.Load<GameObject>("RoomEmpty"), transform);
            SpriteRenderer roomRenderer = newRoomGO.GetComponentInChildren<SpriteRenderer>();
            Transform newTileTrans = newRoomGO.transform.GetChild(0).transform;

            newRoomGO.transform.localPosition = new Vector3Int(room.GetPosition.x * _roomSize.x,
                room.GetPosition.y * _roomSize.y, (int)newRoomGO.transform.localPosition.z);
            roomRenderer.sprite = roomModule.GetRoomSprite;
            newTileTrans.localScale = new Vector3(newTileTrans.localScale.x * _roomSize.x,
                newTileTrans.localScale.y * _roomSize.y, newTileTrans.localScale.z);

            if (room.GetPosition.x == _startRoom.GetPosition.x && room.GetPosition.y == _startRoom.GetPosition.y)
            {
                roomRenderer.color = Color.green;
            }
            else if (room.GetPosition.x == _exitRoom.GetPosition.x && room.GetPosition.y == _exitRoom.GetPosition.y)
            {
                roomRenderer.color = Color.red;
            }

            Tilemap tilemapPrefab = room.GetSelectedRoomPrefab;
            byte[,] tileTypeMap = new byte[_roomSize.x, _roomSize.y];

            for (int x = 0; x < _roomSize.x; x++)
            {
                for (int y = 0; y < _roomSize.y; y++)
                {
                    _environTileMap.SetTile(new Vector3Int(x + room.GetPosition.x * _roomSize.x - _roomSize.x / 2,
                            y + room.GetPosition.y * _roomSize.y - _roomSize.y / 2, (int)_environTileMap.transform.position.z),
                            tilemapPrefab.GetTile(new Vector3Int(x - _roomSize.x / 2, y - _roomSize.y / 2)));
                }
            }
        }
        public void CreateItems(ItemElement[,] itemRoomGrid, RoomElement room)
        {
            for (int x = 0; x < itemRoomGrid.GetLength(0); x++)
            {
                for (int y = 0; y < itemRoomGrid.GetLength(1); y++)
                {
                    ItemModule item = itemRoomGrid[x, y].GetSelectedModule as ItemModule;

                    if (item.GetItemSubType() != 'N')
                    {
                        _itemTileMap.SetTile(new Vector3Int(x + room.GetPosition.x * _roomSize.x - _roomSize.x / 2,
                            y + room.GetPosition.y * _roomSize.y - _roomSize.y / 2, (int)_itemTileMap.transform.position.z),
                            item.GetTileBase);
                    }
                } 
            }
        }
    }
}
