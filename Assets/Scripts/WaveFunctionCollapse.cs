using System.Collections.Generic;
using UnityEngine;

namespace WFC
{
    //Attempt at static WFC class that can be used for different modules/modulesets
    public static class WaveFunctionCollapse
    {
        public static ElementBase[,] WFCGenerate(IModule[] moduleSet, Vector2Int gridSize)
        {
            ElementBase[,] grid = new ElementBase[gridSize.x, gridSize.y];
            List<Vector2Int> unreachedPositions = new List<Vector2Int>();

            for (int y = 0; y < gridSize.y; y++)
            {
                for (int x = 0; x < gridSize.x; x++)
                {
                    Vector2Int position = new Vector2Int(x, y);
                    grid[x, y] = DetermineElementToCreate(moduleSet, position, gridSize);
                    unreachedPositions.Add(position);
                }
            }
            int rng = Random.Range(0, unreachedPositions.Count);

            CollapseElement(grid[unreachedPositions[rng].x, unreachedPositions[rng].y], grid);
            unreachedPositions.RemoveAt(rng);

            while (unreachedPositions.Count > 0)
            {
                ElementBase curElement;
                List<ElementBase> lowEntropyElements = new List<ElementBase>();
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
            }
            return grid;
        }
        private static ElementBase DetermineElementToCreate(IModule[] moduleSet, Vector2Int position, Vector2Int gridSize)
        {
            switch (moduleSet[0])
            {
                case RoomModule:
                    return new _RoomElement(moduleSet, position, gridSize);
                case TileModule:
                    return new _TileElement(moduleSet, position, gridSize);
                case ItemModule:
                    return new _ItemElement(moduleSet, position, gridSize);
            }

            Debug.Log("Module type not found. WFC determining element");
            return null;
        }

        private static void CollapseElement(ElementBase curElement, ElementBase[,] grid)
        {
            curElement.Collapse();

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

                    ElementBase curNeighbour = grid[curX, curY];

                    if (x > 0)
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.East);
                    }
                    else if (x < 0)
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.West);
                    }
                    else if (y > 0)
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.North);
                    }
                    else if (y < 0)
                    {
                        curNeighbour.RemoveOptions(curElement.GetSelectedModule.South);
                    }
                }
            }
        }
    }
}