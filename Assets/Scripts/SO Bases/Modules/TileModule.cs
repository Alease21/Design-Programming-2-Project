using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    //[CreateAssetMenu(menuName = "WFC/Modules/New Tile Module")]
    public class TileModule : ScriptableObject, IModule
    {
        public enum TileType
        {
            Floor,
            Wall,
            Pit
        }
        [SerializeField] private TileType _tileType;
        [SerializeField] private TileBase _tileBase;

        [SerializeField] private TileModule[] _north;
        [SerializeField] private TileModule[] _east;
        [SerializeField] private TileModule[] _south;
        [SerializeField] private TileModule[] _west;

        public TileType GetTileType { get { return _tileType; } }
        public TileBase GetTileBase { get { return _tileBase; } }
        public IModule[] North { get => _north; set => _north = value as TileModule[]; }
        public IModule[] East { get => _east; set => _east = value as TileModule[]; }
        public IModule[] South { get => _south; set => _south = value as TileModule[]; }
        public IModule[] West { get => _west; set => _west = value as TileModule[]; }

        public string GetWallDirections()
        {
            if (_tileType == TileType.Wall)
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