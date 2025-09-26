using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    public class ItemCreator : MonoBehaviour
    {
        [SerializeField] private RoomCreator _roomCreator;

        [SerializeField] private ItemSet _itemSet;
        [SerializeField] private Tilemap _tileMap;
        //[SerializeField] private ItemElement[,] _itemGrid;

        [SerializeField] private List<TileElement[,]> roomTileGrids = new List<TileElement[,]>();

        private void Start()
        {
            _roomCreator = GetComponent<RoomCreator>();
            _roomCreator.TileGenereationDone += GenerateItems;

           // _itemGrid = new ItemElement[_roomCreator.GetRoomSize.x, _roomCreator.GetRoomSize.y];
        }
        public void AddTileGrid(TileElement[,] tileGrid)
        {
            roomTileGrids.Add(tileGrid);
        }

        public void GenerateItems()
        {
            GameObject boundaryEmpty = new GameObject("ItemBoundaries"); //parent to grid
            boundaryEmpty.transform.parent = _tileMap.transform.parent;
            boundaryEmpty.transform.position = Vector3.zero;

            foreach (TileElement[,] tileGrid in roomTileGrids)
            {
                StartCoroutine(CreateItems(tileGrid));
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
                    if (tileGrid[x,y] != null && tileGrid[x,y].GetIsFloor && !tileGrid[x,y].GetIsPit)
                    {
                        itemGrid[x,y] = new ItemElement(_itemSet.itemModules, tileGrid[x,y]);
                        
                        unreachedPositions.Add(itemGrid[x, y].GetPosition);
                    }
                }
            }
            int rng = Random.Range(0, unreachedPositions.Count);
            
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

                rng = Random.Range(0, lowEntropyElements.Count);
                curElement = lowEntropyElements[rng];

                CollapseElement(curElement, itemGrid);
                unreachedPositions.Remove(curElement.GetPosition);
                yield return null;
            }
        }
        private void CollapseElement(ItemElement curElement, ItemElement[,] grid)
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

                    ItemElement curNeighbour = grid[curX, curY];
                    /*
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
                    }*/
                }
            }
        }
    }

    public class ItemElement
    {
        List<ItemModule> _options;
        ItemModule _selectedModule;

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
        }
        public void RemoveOptions(ItemModule[] legalNeighbors)
        {
            List<ItemModule> temp = new List<ItemModule>(legalNeighbors);
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

            tilemap.SetTile((Vector3Int)_adjustedPosition, _selectedModule.tileBase);
            
            GameObject itemBounds = new($"({_adjustedPosition.x},{_adjustedPosition.y})");
            itemBounds.transform.parent = tilemap.transform.parent.Find("ItemBoundaries");
            itemBounds.transform.position = new Vector3(_adjustedPosition.x + 0.5f, _adjustedPosition.y + 0.5f, 0);

            if (_selectedModule.itemType != ItemModule.ItemType.None ||
                _selectedModule.itemType != ItemModule.ItemType.Rocks)
            {
                BoxCollider2D bc = itemBounds.AddComponent<BoxCollider2D>();
                bc.size = Vector2.one;
            }
        }
    }
}