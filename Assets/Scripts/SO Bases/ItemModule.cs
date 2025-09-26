using UnityEngine;
using UnityEngine.Tilemaps;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Item Module")]
    public class ItemModule : ScriptableObject
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

        public ItemModule[] north;
        public ItemModule[] east;
        public ItemModule[] south;
        public ItemModule[] west;

        public char GetTileSubType()
        {
            return name[3];
        }
    }
}