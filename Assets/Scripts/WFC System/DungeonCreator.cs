using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Tilemaps;
using static WFC.WaveFunctionCollapse;

namespace WFC
{
    public class DungeonCreator : MonoBehaviourPunCallbacks
    {
        // Singleton setup
        public static DungeonCreator instance;
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(this.gameObject);

            //Initial neighbor sets in case project didn't load with them correctly
            _roomSet.SetNeighbours();
            _itemTileSet.SetNeighbours();
        }

        [Space(5)]
        [SerializeField] private RoomSet _roomSet;
        [SerializeField] private ItemTileSet _itemTileSet;
        private RoomElement[,] _roomGrid;

        [Header("Map & Room Options")]
        [SerializeField] private Vector2Int _mapSize;
        [SerializeField] private Vector2Int _roomSize;
        private Tilemap _environTileMap;
        private Tilemap _itemTileMap;
        private Transform _boundaryParent;
        private Transform _placeholderMapParent;

        private bool _isStartMade = false,
                     _isExitMade = false;
        private RoomElement _startRoom,
                            _exitRoom;
        private int numPathsOpen = 0,
                    numDungeonTiles = 0;

        private Stopwatch _stopWatch = new Stopwatch();

        [Header("Debug & Editing")]
        [SerializeField] private bool _showTileHighlights = false;
        [SerializeField] private bool _createRoomPathPlaceholders = false;

        public event Action WFCFinished; //currently just used for triggering client side player spawn
                                         //kept in case more needed to be added

        public bool IsStartMade { get { return _isStartMade; } set { _isStartMade = value; } }
        public bool IsExitMade { get { return _isExitMade; } set { _isExitMade = value; } }
        public RoomElement StartRoom { get { return _startRoom; } set { _startRoom = value; } }
        public RoomElement ExitRoom { get { return _exitRoom; } set { _exitRoom = value; } }
        public Vector2Int GetMapSize { get { return _mapSize; } }
        public Vector2Int GetRoomSize { get { return _roomSize; } }

        private void Start()
        {
            _environTileMap = transform.Find("EnvironTilemap").GetComponent<Tilemap>();
            _itemTileMap = transform.Find("ItemTilemap").GetComponent<Tilemap>();
            _boundaryParent = transform.Find("DungeonBoundaries");

            //Placeholder map stuff
            if (_createRoomPathPlaceholders)
                _placeholderMapParent = new GameObject("PlaceholderMap").transform;

            if (PhotonNetwork.IsMasterClient)
            {
                UnityEngine.Random.InitState(NetworkManager.instance.dungeonSeed);
                CollapseRooms();
            }
        }

        [PunRPC]
        public void NonMasterStatup(int seed)
        {
            if (!PhotonNetwork.IsMasterClient)
            {
                NetworkManager.instance.dungeonSeed = seed;
                UnityEngine.Random.InitState(seed);
                CollapseRooms();
            }
        }
        public void ReGenerateSeed()
        {
            NetworkManager.instance.dungeonSeed = UnityEngine.Random.Range(0, int.MaxValue);
            UnityEngine.Random.InitState(NetworkManager.instance.dungeonSeed);
        }
        private void Update()
        {
            // Quick input to regenerate dungeon, remove later
            //if (Input.GetKeyDown(KeyCode.G))
            //RestartGeneration();
        }

        // Displays elapsed time during dungeon generation
        private void GenerationTimer()
        {
            _stopWatch.Stop();
            UnityEngine.Debug.Log($"Genereation Time: {_stopWatch.Elapsed}");
            _stopWatch.Reset();
        }

        // Delete current dungeon rooms, tile, items and restart generation
        private void RestartGeneration()
        {
            ReGenerateSeed();

            StopAllCoroutines();

            //Placeholder map stuff
            if (_placeholderMapParent != null)
                Destroy(_placeholderMapParent.gameObject);
            if (_createRoomPathPlaceholders)
                _placeholderMapParent = new GameObject("PlaceholderMap").transform;

            for (int i = 0; i < _boundaryParent.transform.childCount; i++)
            {
                Destroy(_boundaryParent.transform.GetChild(i).gameObject);
            }
            _environTileMap.ClearAllTiles();
            _itemTileMap.ClearAllTiles();
            _isStartMade = false;
            _isExitMade = false;
            numDungeonTiles = 0;
            numPathsOpen = 0;

            CollapseRooms();
        }
        private void CollapseRooms()
        {
            _stopWatch.Start();
            _roomGrid = new RoomElement[_mapSize.x, _mapSize.y];

            _roomGrid = WFCGenerate(_roomSet.Modules, _mapSize) as RoomElement[,];
            StartCoroutine(SearchPathCoro());
        }

        private void CollapseItems(RoomElement room)
        {
            Vector2Int itemTileGridSize = new Vector2Int((_roomSize.x - 2) / _itemTileSet.GetTrueModuleWidth, (_roomSize.y - 2) / _itemTileSet.GetTrueModuleWidth); // subtract 2 from room dimesions to account for walls
                                                                                                                                                                    // and divide by module width

            ItemElement[,] curItemGrid = WFCGenerate(_itemTileSet.Modules, itemTileGridSize, room) as ItemElement[,];
            CreateItems(curItemGrid, room);
        }

        // Start recursive path search algroithm and wait for result. If no true path found to exit, restart generation,
        // else create placeholder room tiles
        private IEnumerator SearchPathCoro()
        {
            SearchTruePath(_startRoom);
            yield return new WaitUntil(() => numPathsOpen == 0);

            if (_exitRoom == null) { UnityEngine.Debug.Log("Exit room null"); }

            // Regenerate dungeon if number of rooms is greater that 75% of map size,
            // else start creating rooms & items
            if (numDungeonTiles > (0.75f * _mapSize.x * _mapSize.y) || !_exitRoom.IsTruePath)
                RestartGeneration();
            else
            {
                //Camera.main.transform.position = new Vector3(_startRoom.GetPosition.x * _roomSize.x, _startRoom.GetPosition.y * _roomSize.y, -10);

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
                            yield return null;
                        }
                    }
                }
                GenerationTimer();
                WFCFinished?.Invoke();

                if (PhotonNetwork.IsMasterClient)
                    photonView.RPC("NonMasterStatup", RpcTarget.All, NetworkManager.instance.dungeonSeed); //Start other client WFC with true seed
            }
        }

        // Recursive method to search for true path starting from start room. 
        private void SearchTruePath(RoomElement currElement, char trueNeighborDir = 'Z')
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
        private bool CheckIfInBounds(Vector2Int pos)
        {
            if (pos.x >= _mapSize.x || pos.x < 0 || pos.y >= _mapSize.y || pos.y < 0)
                return false;
            return true;
        }

        private void CreateRoom(RoomElement room)
        {
            RoomModule roomModule = room.GetSelectedModule as RoomModule;
            //roomModule.RoomType = (RoomModule.RoomTypes)Random.Range((int)0, 2);

            /*/Set up room transitions for each room REMOVED FOR NOW
            GameObject newRoomTar = Instantiate(Resources.Load<GameObject>("RoomTarget"));
            newRoomTar.transform.localPosition = new Vector3Int(room.GetPosition.x * _roomSize.x,
                room.GetPosition.y * _roomSize.y, (int)newRoomTar.transform.localPosition.z);
            GameObject NorthTrans = newRoomTar.transform.Find("NorthRoomTrans").gameObject,
                       EastTrans = newRoomTar.transform.Find("EastRoomTrans").gameObject,
                       SouthTrans = newRoomTar.transform.Find("SouthRoomTrans").gameObject,
                       WestTrans = newRoomTar.transform.Find("WestRoomTrans").gameObject;
            GameObject[] roomTransitions = { NorthTrans, EastTrans, SouthTrans, WestTrans };

            string roomDirs = roomModule.GetDirString();
            if (roomDirs[0] == '-')
                NorthTrans.SetActive(false);
            if (roomDirs[1] == '-')
                EastTrans.SetActive(false);
            if (roomDirs[2] == '-')
                SouthTrans.SetActive(false);
            if (roomDirs[3] == '-')
                WestTrans.SetActive(false);

            if (room.GetEdgeBool)
            {
                if (room.GetPosition.x == 0 && roomDirs[3] == 'W')
                    WestTrans.SetActive(false);
                if (room.GetPosition.y == 0 && roomDirs[2] == 'S')
                    SouthTrans.SetActive(false);
                if (room.GetPosition.x == _roomGrid.GetLength(0) && roomDirs[1] == 'E')
                    EastTrans.SetActive(false);
                if (room.GetPosition.y == _roomGrid.GetLength(1) && roomDirs[0] == 'N')
                    NorthTrans.SetActive(false);
            }

            foreach (GameObject roomTrans in roomTransitions)
            {
                if (!roomTrans.activeInHierarchy) continue;

                TransTriggerScripts tts = roomTrans.GetComponent<TransTriggerScripts>();
                tts.SetRoomTargets()

            }
            */

            // Create placeholder room path for editing & visual reference 
            if (_createRoomPathPlaceholders)
            {
                GameObject newRoomGO = Instantiate(Resources.Load<GameObject>("RoomEmpty"), _placeholderMapParent);
                SpriteRenderer roomRenderer = newRoomGO.GetComponentInChildren<SpriteRenderer>();
                Transform newTileTrans = newRoomGO.transform.GetChild(0).transform;

                newRoomGO.transform.localPosition = new Vector3Int(room.GetPosition.x * _roomSize.x,
                    room.GetPosition.y * _roomSize.y, (int)newRoomGO.transform.localPosition.z);
                roomRenderer.sprite = roomModule.GetRoomSprite;
                newTileTrans.localScale = new Vector3(newTileTrans.localScale.x * _roomSize.x,
                    newTileTrans.localScale.y * _roomSize.y, newTileTrans.localScale.z);

                if (room.GetPosition.x == _startRoom.GetPosition.x && room.GetPosition.y == _startRoom.GetPosition.y)
                    roomRenderer.color = Color.green;
                else if (room.GetPosition.x == _exitRoom.GetPosition.x && room.GetPosition.y == _exitRoom.GetPosition.y)
                    roomRenderer.color = Color.red;
            }

            Tilemap tilemapPrefab = room.GetSelectedRoomPrefab;

            int playerSpawnsSet = 0;

            for (int x = 0; x < _roomSize.x; x++)
            {
                for (int y = 0; y < _roomSize.y; y++)
                {
                    Vector3Int tilePos = new Vector3Int(x + room.GetPosition.x * _roomSize.x - _roomSize.x / 2,
                            y + room.GetPosition.y * _roomSize.y - _roomSize.y / 2, (int)_environTileMap.transform.position.z);

                    Vector3Int prefabTilePos = new Vector3Int(x - _roomSize.x / 2, y - _roomSize.y / 2);

                    _environTileMap.SetTile(tilePos, tilemapPrefab.GetTile(prefabTilePos));

                    Vector3 spawnedObjPos = (Vector3)tilePos + new Vector3(0.5f, 0.5f, 0f);

                    if (room.GetRoomByteMap[x, y] == 1 || room.GetRoomByteMap[x, y] == 2)
                    {
                        GameObject newTileBoundary = new GameObject($"({x},{y})");
                        newTileBoundary.transform.parent = _boundaryParent;
                        newTileBoundary.transform.position = spawnedObjPos;
                        newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                    }
                    else if (room == _startRoom && playerSpawnsSet < PhotonNetwork.PlayerList.Length &&
                             room.GetRoomByteMap[x, y] == 5)
                    {
                        GameObject playerSpawn = Instantiate(Resources.Load<GameObject>("PlayerSpawn"), spawnedObjPos, Quaternion.identity);
                        GameManager.instance.spawnPoints[playerSpawnsSet] = playerSpawn.transform;
                        playerSpawnsSet++;
                        UnityEngine.Debug.Log("Player Spawn Spawned");
                    }

                    if ((room == _startRoom) && room.GetRoomByteMap[x, y] == 5)
                    {
                        if (x == 0)
                        {
                            GameObject newTileBoundary = new GameObject();
                            newTileBoundary.transform.parent = _boundaryParent;
                            newTileBoundary.transform.position = spawnedObjPos + Vector3.left;
                            newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                        }
                        if (y == 0)
                        {
                            GameObject newTileBoundary = new GameObject();
                            newTileBoundary.transform.parent = _boundaryParent;
                            newTileBoundary.transform.position = spawnedObjPos + Vector3.down;
                            newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                        }
                        if (x == _roomGrid.GetLength(0) - 1)
                        {
                            GameObject newTileBoundary = new GameObject();
                            newTileBoundary.transform.parent = _boundaryParent;
                            newTileBoundary.transform.position = spawnedObjPos + Vector3.right;
                            newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                        }
                        if (y == _roomGrid.GetLength(1) - 1)
                        {
                            GameObject newTileBoundary = new GameObject();
                            newTileBoundary.transform.parent = _boundaryParent;
                            newTileBoundary.transform.position = spawnedObjPos + Vector3.up;
                            newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                        }
                    }

                    if (!_showTileHighlights) continue; // skip tile coloring if bool not checked

                    _environTileMap.SetTileFlags(tilePos, TileFlags.None);

                    if (room.GetRoomByteMap[x, y] == 3 || room.GetRoomByteMap[x, y] == 4)
                        _environTileMap.SetColor(tilePos, Color.blue + new Color(0.5f, 0.5f, 0f));
                    else if (room.GetRoomByteMap[x, y] == 5)
                    {
                        if (_startRoom == room)
                            _environTileMap.SetColor(tilePos, Color.green);
                        else if (_exitRoom == room)
                            _environTileMap.SetColor(tilePos, Color.red);
                    }
                }
            }
        }

        private void CreateItems(ItemElement[,] itemRoomGrid, RoomElement room)
        {
            //loop through grid of tile modules
            for (int x = 0; x < itemRoomGrid.GetLength(0); x++)
            {
                for (int y = 0; y < itemRoomGrid.GetLength(1); y++)
                {
                    ItemTileModule itemTile = itemRoomGrid[x, y].GetSelectedModule as ItemTileModule;

                    // Loop through tiles in module area
                    for (int i = 0; i < _itemTileSet.GetTrueModuleWidth; i++)
                    {
                        for (int j = 0; j < _itemTileSet.GetTrueModuleWidth; j++)
                        {
                            int tileIndex = itemTile.GetTrueTileIndex(i, j);
                            TileBase tile = itemTile.GetTrueTiles[tileIndex];
                            Vector3Int moduleOriginRoom = new Vector3Int(x * _itemTileSet.GetTrueModuleWidth, y * _itemTileSet.GetTrueModuleWidth);
                            Vector3Int moduleOriginWorld = new Vector3Int(moduleOriginRoom.x + room.GetPosition.x * _roomSize.x - _roomSize.x / 2,
                                                                          moduleOriginRoom.y + room.GetPosition.y * _roomSize.y - _roomSize.y / 2); // +1 to account for walls in room

                            if (room.GetRoomByteMap[moduleOriginRoom.x + i, moduleOriginRoom.y + j] != 0) continue; //if tile is non-floor, do not place anything
                            if (tile == null) continue;// if no tile base, continue

                            //check if it is multi tile object (naming convention)
                            if (tile.name[0] == 'M')
                            {
                                string connectionDirs = tile.name.Substring(tile.name.Length - 4);
                                Vector2Int nNeighbour = new Vector2Int(moduleOriginRoom.x + i, moduleOriginRoom.y + j) + Vector2Int.up,
                                           eNeighbour = new Vector2Int(moduleOriginRoom.x + i, moduleOriginRoom.y + j) + Vector2Int.right,
                                           sNeighbour = new Vector2Int(moduleOriginRoom.x + i, moduleOriginRoom.y + j) + Vector2Int.down,
                                           wNeighbour = new Vector2Int(moduleOriginRoom.x + i, moduleOriginRoom.y + j) + Vector2Int.left;

                                // if multi tile object overlaps non floor tile, do not place anything/continue loop
                                if (connectionDirs[0] == 'N' && room.GetRoomByteMap[nNeighbour.x, nNeighbour.y] != 0 ||
                                    connectionDirs[1] == 'E' && room.GetRoomByteMap[eNeighbour.x, eNeighbour.y] != 0 ||
                                    connectionDirs[2] == 'S' && room.GetRoomByteMap[sNeighbour.x, sNeighbour.y] != 0 ||
                                    connectionDirs[3] == 'W' && room.GetRoomByteMap[wNeighbour.x, wNeighbour.y] != 0)
                                    continue;
                            }

                            if (tile.name[1] == 'B' && tile.name[tile.name.Length - 1] == '0') // chest tile
                            {
                                GameObject newChest = Instantiate(Resources.Load<GameObject>("Chest"));
                                newChest.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.5f, 0f);

                                // spawn collider seperate from chest item for trigger enter functions
                                GameObject newTileBoundary = new GameObject($"({x},{y})");
                                newTileBoundary.transform.parent = _boundaryParent;
                                newTileBoundary.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.5f, 0f);
                                newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;

                                continue;
                            }
                            else if (tile.name[1] == 'P' && tile.name[tile.name.Length - 1] == '5') // coin tile
                            {
                                GameObject newCoin = Instantiate(Resources.Load<GameObject>("Coin"));
                                newCoin.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.11f, 0f);
                                continue;
                            }
                            else if (tile.name == "Props_78")// Enemy tile **rename me
                            {
                                GameObject newEnemy = Instantiate(Resources.Load<GameObject>("Enemy"));
                                newEnemy.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.5f, 0f);
                                continue;
                            }
                            else if (tile.name == "Props_76")// Health pot tile **rename me
                            {
                                GameObject newHealthPot = Instantiate(Resources.Load<GameObject>("HealthPotion"));
                                newHealthPot.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0f, 0f);
                                continue;
                            }
                            /* removed merchant for now, couldn't get gui to work right/didnt have time
                            else if (tile.name == "Props_74")// Merchant tile **rename me
                            {
                                GameObject newMerchant = Instantiate(Resources.Load<GameObject>("ShopKeep"));
                                newMerchant.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.5f, 0f);
                                newMerchant.AddComponent<BoxCollider>().size = Vector2.one;
                                continue;
                            }
                            */

                            if (tile.name[3] == 'C')
                            {
                                GameObject newTileBoundary = new GameObject($"({x},{y})");
                                newTileBoundary.transform.parent = _boundaryParent;
                                newTileBoundary.transform.position = moduleOriginWorld + new Vector3Int(i, j) + new Vector3(0.5f, 0.5f, 0f);
                                newTileBoundary.AddComponent<BoxCollider>().size = Vector2.one;
                            }

                            _itemTileMap.SetTile(moduleOriginWorld + new Vector3Int(i, j), tile);
                        }
                    }
                }
            }
        }
    }
}
