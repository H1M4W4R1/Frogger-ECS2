using Player.Components;
using Threats.Components;
using Threats.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Threats.Systems
{
    public partial struct ThreatMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MovingThreat>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Scan for platform movement (cancel if player is in jump)
            var movePlatforms = true;
            foreach (var (localTransform, threat, e)
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MovingThreat>>().WithEntityAccess())
            {
                // If platform
                if (SystemAPI.HasComponent<IsPlatform>(e))
                {
                    // With animating player then cancel move
                    if (SystemAPI.IsComponentEnabled<IsPlayerOnPlatform>(e))
                    {
                        var player = SystemAPI.GetSingletonEntity<PlayerTag>();
                        var playerMovementInfo = SystemAPI.GetComponent<CurrentMovementData>(player);

                        // Stun platform if player is jumping onto it ;) [better UX]
                        if (playerMovementInfo.isJumpAnimating || playerMovementInfo.isMovementComputing)
                            movePlatforms = false;
                    }
                }
            }
                
            // Try to move
            foreach (var (localTransform, threat, e) 
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRW<MovingThreat>>().WithEntityAccess())
            {
                var tData = threat.ValueRO;
                var vData = localTransform.ValueRW.Position + tData.direction * tData.speed * SystemAPI.Time.DeltaTime;
      
                if (movePlatforms)
                {
                    localTransform.ValueRW.Position = vData;
                    threat.ValueRW.currentPosition = vData;
                }
            }

            /*var job = AcquireNearestPlatformJob.Prepare();
            job.Schedule(state.Dependency).Complete();
            
            // Dispose useless array
            state.Dependency = job.Dispose(state.Dependency);*/
        }
    }
}