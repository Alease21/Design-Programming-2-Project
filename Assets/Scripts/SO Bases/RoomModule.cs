using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Room Module")]
    public class RoomModule : ScriptableObject
    {
        public Vector2Int roomSize;
        public Sprite roomSprite;

        public RoomModule[] north;
        public RoomModule[] east;
        public RoomModule[] south;
        public RoomModule[] west;
    }
}