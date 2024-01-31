using Unity.Entities;

namespace Levels.Components
{
    public struct RenderedLevelTile : IBufferElementData
    {
        public byte tileId;
        
        public int xPosition;
        public int zPosition;
    }
}