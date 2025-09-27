using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    public class ItemCreator : MonoBehaviour
    {
        //Singleton setup
        public static ItemCreator instance;
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

            _itemSet.SetNeighbours();
        }
        //

        [SerializeField] private ItemSet _itemSet;
        [SerializeField] private Tilemap _tileMap;

        private List<TileElement[,]> roomTileGrids = new List<TileElement[,]>();
        private Vector2Int _numRoomsCompleted; // used for tracking room tile generation (done/max)

        public Action ItemGenerationDone;

        private void Start()
        {
            RoomCreator.instance.TileGenereationDone += GenerateItems;
        }
        public void AddTileGrid(TileElement[,] tileGrid)
        {
            roomTileGrids.Add(tileGrid);
        }
        public void ClearTiles()
        {
            _tileMap.ClearAllTiles();
        }
        public void GenerateItems()
        {
            GameObject boundaryEmpty = new GameObject("ItemBoundaries"); //parent to grid
            boundaryEmpty.transform.parent = _tileMap.transform.parent;
            boundaryEmpty.transform.position = Vector3.zero;

            foreach (TileElement[,] tileGrid in roomTileGrids)
            {
                StartCoroutine(CreateItems(tileGrid));
                _numRoomsCompleted.y++;
            }
        }

        public IEnumerator CreateItems(TileElement[,] tileGrid)
        {
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();
            ItemElement[,] itemGrid = new ItemElement[tileGrid.GetLength(0), tileGrid.GetLength(1)];

            for (int x = 0; x < tileGrid.GetLength(0); x++)
            {
                for (int y = 0; y < tileGrid.GetLength(1); y++)
                {
                    if (!tileGrid[x,y].GetIsPit)
                    {
                        itemGrid[x,y] = new ItemElement(_itemSet.itemModules, tileGrid[x,y]);
                        
                        unreachedPositions.Add(itemGrid[x, y].GetPosition);
                    }
                }
            }
            int rng = UnityEngine.Random.Range(0, unreachedPositions.Count);
            
            CollapseElement(itemGrid[unreachedPositions[rng].x, unreachedPositions[rng].y], itemGrid);
            unreachedPositions.RemoveAt(rng);

            while (unreachedPositions.Count > 0)
            {
                ItemElement curElement;
                List<ItemElement> lowEntropyElements = new List<ItemElement>();
                int lowestEntropy = int.MaxValue;

                for (int i = 0; i < unreachedPositions.Count; i++)
                {
                    curElement = itemGrid[unreachedPositions[i].x, unreachedPositions[i].y];
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

                CollapseElement(curElement, itemGrid);
                unreachedPositions.Remove(curElement.GetPosition);
                yield return null;
            }

            _numRoomsCompleted.x++;
            if (_numRoomsCompleted.x == _numRoomsCompleted.y)
                ItemGenerationDone?.Invoke();
        }
        private void CollapseElement(ItemElement curElement, ItemElement[,] grid)
        {
            if (curElement == null) return;
            curElement.Collapse(_tileMap);
            
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if ((x == 0 && y == 0) || (Mathf.Abs(x) == 1 && Mathf.Abs(y) == 1)) // setting up for neighbors, but not diags (1,1)
                        continue;

                    int curX = curElement.GetPosition.x + x;
                    int curY = curElement.GetPosition.y + y;

                    if ((curX < 0 || curY < 0) || (curX > grid.GetLength(0) - 1 || curY > grid.GetLength(1) - 1))
                        continue;

                    ItemElement curNeighbour = grid[curX, curY];
                    if (curNeighbour == null) continue;

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
    }

    public class ItemElement
    {
        List<ItemModule> _options;
        ItemModule _selectedModule;
        TileModule.TileType _tileType;

        Vector2Int _position;
        Vector2Int _adjustedPosition;
        Vector2Int _roomSize;

        public Vector2Int GetPosition { get { return _position; } }
        public Vector2Int GetAdjustedPosition { get { return _adjustedPosition; } }
        public Vector2Int GetRoomSize { get { return _roomSize; } }
        public int GetEntropy { get { return _options.Count; } }
        public ItemModule GetSelectedModule { get { return _selectedModule; } }

        public ItemElement(ItemModule[] options, TileElement tile)
        {
            _position = tile.GetPosition;
            _adjustedPosition = tile.GetAdjustedPosition;
            _roomSize = tile.GetRoomSize;
            _options = new List<ItemModule>(options);
            _tileType = tile.GetSelectedModule.tileType;
        }
        public void RemoveOptions(ItemModule[] legalNeighbors)
        {
            List<ItemModule> temp = new List<ItemModule>(legalNeighbors);
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (temp.Contains(_options[i]) == false)
                    _options.RemoveAt(i);
            }
        }
        public void RemoveOptions(ItemModule.ItemType illegaltype)
        {
            for (int i = _options.Count - 1; i >= 0; i--)
            {
                if (_options[i].itemType == illegaltype)
                    _options.RemoveAt(i);
            }
        }
        public void Collapse(Tilemap tilemap)
        {
            switch ((int)_tileType)
            {
                case 0: //floor
                    RemoveOptions(ItemModule.ItemType.Torch);
                    RemoveOptions(ItemModule.ItemType.Banner);

                    if (_position.y == 0 || _position.y == _roomSize.y - 1 ||
                        _position.x == 0 || _position.x == _roomSize.x - 1)
                    {
                        RemoveOptions(ItemModule.ItemType.Table);
                        RemoveOptions(ItemModule.ItemType.Box);
                        RemoveOptions(ItemModule.ItemType.Rocks);
                        RemoveOptions(ItemModule.ItemType.Chair);
                    }
                    if (_position.y > 2 && _position.y < _roomSize.y - 3 &&
                        _position.x > 2 && _position.x < _roomSize.x - 3)
                    {
                        RemoveOptions(ItemModule.ItemType.Table);
                    }

                        break;
                case 1: //wall
                    RemoveOptions(ItemModule.ItemType.Table);
                    RemoveOptions(ItemModule.ItemType.Box);
                    RemoveOptions(ItemModule.ItemType.Rocks);
                    RemoveOptions(ItemModule.ItemType.Chair);

                    if (_position.y != _roomSize.y - 1 || 
                        _position.x == 0 || _position.x == _roomSize.x - 1)
                    {
                        RemoveOptions(ItemModule.ItemType.Torch);
                        RemoveOptions(ItemModule.ItemType.Banner);
                    }
                    break;
                case 2: //pit
                    //currently initial item grid setup for excluding pits
                    break;
            }
            int itemRNG = UnityEngine.Random.Range(0, 100);
            if (itemRNG <= 80)
            {
                RemoveOptions(ItemModule.ItemType.Table);
                RemoveOptions(ItemModule.ItemType.Box);
                RemoveOptions(ItemModule.ItemType.Rocks);
                RemoveOptions(ItemModule.ItemType.Chair);
            }
            else
            {
                if (_options.Count > 1)
                    RemoveOptions(ItemModule.ItemType.None);
            }
            int rng = UnityEngine.Random.Range(0, _options.Count);
            _selectedModule = _options[rng];

            tilemap.SetTile((Vector3Int)_adjustedPosition, _selectedModule.tileBase);
            
            GameObject itemBounds = new($"({_adjustedPosition.x},{_adjustedPosition.y})");
            itemBounds.transform.parent = tilemap.transform.parent.Find("ItemBoundaries");
            itemBounds.transform.position = new Vector3(_adjustedPosition.x + 0.5f, _adjustedPosition.y + 0.5f, 0);

            if (_selectedModule.itemType != ItemModule.ItemType.None &&
                _selectedModule.itemType != ItemModule.ItemType.Rocks)
            {
                BoxCollider2D bc = itemBounds.AddComponent<BoxCollider2D>();
                bc.size = Vector2.one;
            }
        }
    }
}