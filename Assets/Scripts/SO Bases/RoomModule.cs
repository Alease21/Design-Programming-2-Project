using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Room Module")]
    public class RoomModule : ScriptableObject, IModule
    {
        public enum RoomType
        {
            None,
            PitRoom,
            //TilePatternRoom,
            //TableRoom
        }
        public RoomType roomType;
        public Vector2Int roomSize;
        public Sprite roomSprite;

        private RoomModule[] _north;
        private RoomModule[] _east;
        private RoomModule[] _south;
        private RoomModule[] _west;

        public IModule[] North { get => _north; set => value = _north; }
        public IModule[] East { get => _north; set => value = _north; }
        public IModule[] South { get => _north; set => value = _north; }
        public IModule[] West { get => _north; set => value = _north; }

        public string GetDirString()
        {
            return name.Substring(name.Length - 4);
        }
    }
}