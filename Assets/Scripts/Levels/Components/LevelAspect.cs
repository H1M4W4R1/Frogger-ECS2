using Unity.Entities;

namespace Levels.Components
{
    public readonly partial struct LevelAspect : IAspect
    {
        // Tags
        public readonly RefRO<LevelTag> levelTag;
        
        // Data
        // public readonly RefRW<LevelBuiltTag> isLevelBuilt; // Inside main request
        public readonly RefRO<LevelData> levelData;

    }
}