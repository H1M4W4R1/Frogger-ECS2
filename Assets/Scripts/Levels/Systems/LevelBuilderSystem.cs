using Levels.Components;
using Unity.Burst;
using Unity.Entities;

namespace Levels.Systems
{
    public partial struct LevelBuilderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelTag>();
            state.RequireForUpdate<LevelBuiltTag>();
            state.RequireForUpdate<LevelData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Lookup for build tags
            
            foreach (var (loadedLevel, entity) in SystemAPI.Query<LevelAspect>().WithDisabled<LevelBuiltTag>().WithEntityAccess())
            {
                var levelData = loadedLevel.levelData.ValueRO;
                
                // TODO: Build level data
                
                SystemAPI.SetComponentEnabled<LevelBuiltTag>(entity, true);
            }
        }
    }
}