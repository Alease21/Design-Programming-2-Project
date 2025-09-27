using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Tile Module")]
    public class TileModule : ScriptableObject, IModule
    {
        public enum TileType
        {
            Floor,
            Wall,
            Pit
        }
        public TileType tileType;
        public TileBase tileBase;

        private TileModule[] _north;
        private TileModule[] _east;
        private TileModule[] _south;
        private TileModule[] _west;

        public IModule[] North { get => _north; set => value = _north; }
        public IModule[] East { get => _north; set => value = _east; }
        public IModule[] South { get => _north; set => value = _south; }
        public IModule[] West { get => _north; set => value = _west; }

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