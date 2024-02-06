using Levels.Components;
using Levels.Systems.Jobs;
using Player.Aspects;
using Player.Components;
using Threats.Components;
using Threats.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Levels.Systems
{
    public partial struct LevelManagementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (player, e) in SystemAPI.Query<PlayerAspect>().WithEntityAccess())
            {
                // Get dead player for current level
                if (SystemAPI.IsComponentEnabled<IsDead>(e) && 
                    SystemAPI.TryGetSingleton<LevelData>(out var lData))
                {
                    // Remove nearest platform player handling state
                    /*AcquireNearestPlatformJob.Prepare(out var nearestPlatformJob,
                        player.localTransform.ValueRO.Position);
                    nearestPlatformJob.Schedule(state.Dependency).Complete();

                    var pEntity = nearestPlatformJob.platformEntity[0];
                    if(SystemAPI.Exists(pEntity))
                        SystemAPI.SetComponentEnabled<IsPlayerOnPlatform>(pEntity, false);
                    
                    nearestPlatformJob.Dispose();*/
                    
                    // Remove platform states
                    foreach (var (platform, platformEntity) in SystemAPI.Query<MovingThreat>()
                                 .WithAll<IsPlatform, IsPlayerOnPlatform>().WithEntityAccess())
                    {
                        SystemAPI.SetComponentEnabled<IsPlayerOnPlatform>(platformEntity, false);
                    }
                    
                    // Move player to start and make him alive
                    ResetPlayerPositionToStart.Prepare(out var job, lData);
                    job.Schedule(state.Dependency).Complete();

                    // Disable platform sticking
                    if (SystemAPI.IsComponentEnabled<IsOnPlatform>(e))
                        SystemAPI.SetComponentEnabled<IsOnPlatform>(e, false);
                    
                    SystemAPI.SetComponentEnabled<IsDead>(e, false);

                    job.Dispose();
                }
            }
        }
    }
}