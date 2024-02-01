using Unity.Entities;

namespace Levels.Components
{
    public struct RenderedLevelTile : IComponentData
    {
        public byte tileId;
        
        public int xPosition;
        public int zPosition;
    }
}