using Unity.Entities;

namespace Levels.Components
{
    public struct RenderedLevelTile : IComponentData
    {
        public byte tileId;
        public bool isKillTile;
        
        public int xPosition;
        public int zPosition;
    }
}