using Levels.Components;
using Levels.Systems.Jobs;
using Player.Components;
using Unity.Burst;
using Unity.Entities;

namespace Levels.Systems
{
    public partial struct LevelManagementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            
            foreach (var (player, e) in SystemAPI.Query<PlayerTag>().WithEntityAccess())
            {
                if (SystemAPI.IsComponentEnabled<IsDead>(e) && 
                    SystemAPI.TryGetSingleton<LevelData>(out var lData))
                {
                    ResetPlayerPositionToStart.Prepare(out var job, lData);
                    job.Schedule();

                    SystemAPI.SetComponentEnabled<IsDead>(e, false);
                }
            }
        }
    }
}