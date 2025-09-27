using UnityEngine;
using System.Collections.Generic;

namespace WFC
{
    public class _RoomElement : ElementBase
    {
        //isFloor
        //isPit
        //adjusted pos?

        public _RoomElement(IModule[] options, Vector2Int position, Vector2Int mapSize)
        {
            _options = new List<IModule>(options);
            _position = position;
            _gridSize = mapSize;
        }

        public override void Collapse()
        {
            //
        }
    }

    public class _TileElement : ElementBase
    {
        public _TileElement(IModule[] options, Vector2Int position, Vector2Int mapSize)
        {
            _options = new List<IModule>(options);
            _position = position;
            _gridSize = mapSize;
        }
        public override void Collapse()
        {
            //
        }
    }

    public class _ItemElement : ElementBase
    {
        public _ItemElement(IModule[] options, Vector2Int position, Vector2Int mapSize)
        {
            _options = new List<IModule>(options);
            _position = position;
            _gridSize = mapSize;
        }

        public override void Collapse()
        {
            //
        }
    }
}