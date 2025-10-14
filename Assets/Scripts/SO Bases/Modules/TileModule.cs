using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Tile Module")]
    public class TileModule : ScriptableObject, IModule
    {
        public enum TileType
        {
            //
        }
        //[SerializeField] private TileType _tileType;
        [SerializeField] private Tilemap _tilePrefab;
        private Vector2Int _tileSize = new Vector2Int(4,4);// here just so i don't have magic numbers in code? maybe change

        [SerializeField] private TileModule[] _north;
        [SerializeField] private TileModule[] _east;
        [SerializeField] private TileModule[] _south;
        [SerializeField] private TileModule[] _west;

        //public TileType GetTileType { get { return _tileType; } }
        public Tilemap GetTilePrefab { get { return _tilePrefab; } }
        public Vector2Int TileSize { get { return _tileSize; } }
        public IModule[] North { get => _north; set => _north = value as TileModule[]; }
        public IModule[] East { get => _east; set => _east = value as TileModule[]; }
        public IModule[] South { get => _south; set => _south = value as TileModule[]; }
        public IModule[] West { get => _west; set => _west = value as TileModule[]; }
    }
}