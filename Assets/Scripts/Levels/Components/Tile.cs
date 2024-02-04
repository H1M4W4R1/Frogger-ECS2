using Unity.Entities;

namespace Levels.Components
{
    public struct Tile : IComponentData
    {
        public byte tileId;
        public bool isKillTile;
        public bool isKillZone;
        
        public int xPosition;
        public int zPosition;
    }
}