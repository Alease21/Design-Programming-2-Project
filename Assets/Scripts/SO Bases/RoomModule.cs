using UnityEngine;

namespace WFC
{
    [CreateAssetMenu(menuName = "WFC/Modules/New Room Module")]
    public class RoomModule : ScriptableObject, IModule
    {
        public enum RoomTypes
        {
            None,
            PitRoom,
            //TilePatternRoom,
            //TableRoom
        }
        [SerializeField] private RoomTypes _roomType;
        [SerializeField] private Sprite _roomSprite;

        [SerializeField] private RoomModule[] _north;
        [SerializeField] private RoomModule[] _east;
        [SerializeField] private RoomModule[] _south;
        [SerializeField] private RoomModule[] _west;

        public RoomTypes RoomType { get { return _roomType; } set { _roomType = value; } }
        public Sprite GetRoomSprite { get { return _roomSprite; } }
        public IModule[] North { get => _north; set => _north = value as RoomModule[]; }
        public IModule[] East { get => _east; set => _east = value as RoomModule[]; }
        public IModule[] South { get => _south; set => _south = value as RoomModule[]; }
        public IModule[] West { get => _west; set => _west = value as RoomModule[]; }

        public string GetDirString()
        {
            return name.Substring(name.Length - 4);
        }
    }
}