using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Tile Module")]
    public class TileModule : ScriptableObject
    {
        public TileBase tileBase;

        public TileModule[] north;
        public TileModule[] east;
        public TileModule[] south;
        public TileModule[] west;

        public string GetDirString()
        {
            return name.Substring(name.Length - 4);
        }
        public char GetTileTypeChar()
        {
            return name[1];
        }
    }
}