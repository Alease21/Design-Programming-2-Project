using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Tilemaps;
using WFC;

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

            _roomSet.SetNeighbours();
            _tileSet.SetNeighbours();
            _itemSet.SetNeighbours();
        }

        [SerializeField] private RoomSet _roomSet;
        [SerializeField] private TileSet _tileSet;
        [SerializeField] private ItemSet _itemSet;
        [SerializeField] Vector2Int _mapSize;
        [SerializeField] Vector2Int _roomSize;
        [SerializeField] Tilemap _environTileMap;
        [SerializeField] Tilemap _itemTileMap;

        private _RoomElement _startRoom,
                             _exitRoom;
        private bool _exitMade = false;
        private int numPathsOpen = 0,
                    numDungeonTiles = 0;

        private Stopwatch _stopWatch = new Stopwatch();

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
            _exitMade = false;
            numDungeonTiles = 0;
            numPathsOpen = 0;

            //Generate();
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


        public bool CheckIfInBounds(Vector2Int pos)
        {
            if (pos.x >= _mapSize.x || pos.x < 0 || pos.y >= _mapSize.y || pos.y < 0)
                return false;
            return true;
        }

    }
}
