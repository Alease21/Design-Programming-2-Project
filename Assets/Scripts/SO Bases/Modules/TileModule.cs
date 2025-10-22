using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "TileModule", menuName = "Scriptable Objects/TileModule")]
public class TileModule : ScriptableObject
{
    public Vector2Int keyDepthByModuleWidth;

    public TileModule[] northNeighbours;
    public TileModule[] eastNeighbours;
    public TileModule[] southNeighbours;
    public TileModule[] westNeighbours;

    [SerializeField] public TileBase[] trueTiles;
    [SerializeField] public TileBase[] nKey, eKey, sKey, wKey;
    
    
    public void SetBasesAndKeys(TileBase[] tileArray)
    {
        int keyDepth = keyDepthByModuleWidth.x;
        int width = keyDepthByModuleWidth.y;
        List<TileBase> n = new(), e = new(), s = new(), w = new();
        List<TileBase> trueTileList = new();

        /*/
         *  index calc is i = y * w + x
         *  --------------
         *  % gives x 
         *      (width) for min x
         *      (width - 1) for max x
         *  / gives y
         *      (width) for min y
         *      (width - 1) for max y
        /*/
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

        nKey = n.ToArray();
        eKey = e.ToArray();
        sKey = s.ToArray();
        wKey = w.ToArray();
        trueTiles = trueTileList.ToArray();
    }
}
