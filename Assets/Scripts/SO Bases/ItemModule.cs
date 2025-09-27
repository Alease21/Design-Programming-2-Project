using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Item Module")]
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
        public ItemType itemType;
        public TileBase tileBase;

        private ItemModule[] _north;
        private ItemModule[] _east;
        private ItemModule[] _south;
        private ItemModule[] _west;

        public IModule[] North { get => _north; set => value = _north; }
        public IModule[] East { get => _north; set => value = _north; }
        public IModule[] South { get => _north; set => value = _north; }
        public IModule[] West { get => _north; set => value = _north; }

        public char GetTileSubType()
        {
            return name[3];
        }
    }
}