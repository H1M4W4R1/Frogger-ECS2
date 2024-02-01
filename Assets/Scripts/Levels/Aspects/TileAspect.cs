using Levels.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Levels.Aspects
{
    public readonly partial struct TileAspect : IAspect
    {
        public readonly RefRO<LocalTransform> localTransform;
        public readonly RefRO<Tile> tile;
        public readonly RefRW<RenderedLevelTile> render;
    }
}