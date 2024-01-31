using Unity.Entities;

namespace Levels.Components
{
    public struct TileLibrary : IBufferElementData
    {
        public byte index;
        public Entity tile;
    }
}