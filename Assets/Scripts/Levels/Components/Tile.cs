using Unity.Entities;

namespace Levels.Components
{
    public struct Tile : IComponentData
    {
        public byte tileType;
        public bool isKillTile;
    }
}