using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Tile Module")]
    public class TileModule : ScriptableObject
    {
        public enum TileType
        {
            Floor,
            Wall,
            Pit
        }
        public TileType tileType;
        public TileBase tileBase;

        public TileModule[] north;
        public TileModule[] east;
        public TileModule[] south;
        public TileModule[] west;

        public string GetWallDirections()
        {
            if (tileType == TileType.Wall)
                return name.Substring(name.Length - 4);
            else 
            {
                Debug.Log($"Direction string call invalid for {name}");
                return "----";
            }
        }
        public char GetTileSubType()
        {
            return name[2];
        }
    }
}