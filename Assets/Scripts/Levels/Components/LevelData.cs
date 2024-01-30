using Unity.Entities;

namespace Levels.Components
{
    public struct LevelData : IComponentData
    {
        public float time;
        
        // Game speed (time multiplier to determine hardness)
        public float threatSpeed;
    }
}