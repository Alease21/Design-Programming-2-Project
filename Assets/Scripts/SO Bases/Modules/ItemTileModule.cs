using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileModule", menuName = "Scriptable Objects/TileModule")]
public class ItemTileModule : ScriptableObject
{
    public Vector2Int keyDepthByModuleWidth;

    public ItemTileModule[] northNeighbours;
    public ItemTileModule[] eastNeighbours;
    public ItemTileModule[] southNeighbours;
    public ItemTileModule[] westNeighbours;

    [SerializeField] public TileBase[] trueTiles;
    [SerializeField] public TileBase[] nKey, eKey, sKey, wKey;

    // testing
    [SerializeField] TileBase[] tileArrayViewing;
    //
    public void SetBasesAndKeys(TileBase[] tileArray)
    {
        // testing
        tileArrayViewing = tileArray;
        //

        int keyDepth = keyDepthByModuleWidth.x;
        int width = keyDepthByModuleWidth.y - 1;
        List<TileBase> n = new(), e = new(), s = new(), w = new();
        List<TileBase> trueTileList = new();

        int topRight = GetIndex(width, width);
        int topLeft = GetIndex(0, width);

        /*/
         *  index calc is i = y * w + x
         *  --------------
         *  % gives x 
         *      (width) for min x
         *      (width - 1) for max x
         *  / gives y
         *      (width) for min y
         *      (width - 1) for max y
        //
        for (int i = 0; i < tileArray.Length; i++)
        {
            if ((i % width < keyDepth || i % (width - 1) > (width - keyDepth - 1)) &&
                 (i / width < keyDepth || i / (width - 1) > (width - keyDepth - 1)))
                continue;
            else if (i / (width - 1) > (width - keyDepth - 1))
                n.Add(tileArray[i]);
            else if (i % (width - 1) > (width - keyDepth - 1))
                e.Add(tileArray[i]);
            else if (i / width < keyDepth)
                s.Add(tileArray[i]);
            else if (i % width < keyDepth)
                w.Add(tileArray[i]);
            else
                trueTileList.Add(tileArray[i]);
        }
        */
        for (int y = 0; y <= width; y++)
        {
            for (int x = 0; x <= width; x++)
            {
                int index = GetIndex(x, y);
                Debug.Log($"index: {index} ({x},{y})");

                if (index == 0 || index == width || index == topLeft || index == topRight)
                    continue;

                if (y == width)
                    n.Add(tileArray[index]);
                else if (x == width)
                    e.Add(tileArray[index]);
                else if (y == 0)
                    s.Add(tileArray[index]);
                else if (x == 0)
                    w.Add(tileArray[index]);
                else
                    trueTileList.Add(tileArray[index]);
            }
        }

        nKey = n.ToArray();
        eKey = e.ToArray();
        sKey = s.ToArray();
        wKey = w.ToArray();
        trueTiles = trueTileList.ToArray();
    }
    public int GetIndex(int x, int y)
    {
        return y * keyDepthByModuleWidth.y + x;
    }
}
