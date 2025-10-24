using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    /*
    //[CreateAssetMenu(menuName = "WFC/Modules/New Item Module")]
    public class ItemModule : ScriptableObject, IModule
    {
        public enum ItemType
        {
            None,
            Box,
            Table,
            Chair,
            Rocks,
            Torch,
            Banner
        }
        [SerializeField] private ItemType _itemType;
        [SerializeField] private TileBase _tileBase;

        [SerializeField] private ItemModule[] _north;
        [SerializeField] private ItemModule[] _east;
        [SerializeField] private ItemModule[] _south;
        [SerializeField] private ItemModule[] _west;

        public ItemType GetItemType { get { return _itemType; } }
        public TileBase GetTileBase { get { return _tileBase; } }
        public IModule[] North { get => _north; set => _north = value as ItemModule[]; }
        public IModule[] East { get => _east; set => _east = value as ItemModule[]; }
        public IModule[] South { get => _south; set => _south = value as ItemModule[]; }
        public IModule[] West { get => _west; set => _west = value as ItemModule[]; }

        public string GetItemSubType()
        {
            return $"{name[4]}{name[5]}";
        }
        public char GetItemLocationType()
        {
            return name[2];
        }
    }*/
}