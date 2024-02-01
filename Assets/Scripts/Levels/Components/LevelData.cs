using Unity.Entities;

namespace Levels.Components
{
    public struct LevelData : IComponentData
    {
        public float time;

        // Amount of tiles to render to side (eg. 16 tiles to side)
        public int levelHalfRenderedWidth;

        // Amount of tiles player can move on (eg. 3 tiles to side)
        public int levelHalfPlayableWidth;
    }
}