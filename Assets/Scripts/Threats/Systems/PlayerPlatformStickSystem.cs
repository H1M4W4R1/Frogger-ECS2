using Player.Components;
using Player.Systems.Jobs;
using Threats.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Threats.Systems
{
    public partial struct PlayerPlatformStickSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (lThreat, threatTransform, tEntity) in 
                     SystemAPI.Query<RefRO<MovingThreat>, RefRO<LocalTransform>>().WithAll<IsPlatform, IsPlayerOnPlatform>().WithEntityAccess())
            {
                foreach (var lTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
                {
                    var pos = lTransform.ValueRO.Position;
                    
                    // Get offsets at current platform
                    var offsets =
                        SystemAPI.GetBuffer<PlatformOffsetsStore>(tEntity);
                    
                    AcquireNearestPlatformOffset.Prepare(out var job, offsets, pos, threatTransform.ValueRO.Position);
                    job.Schedule().Complete();
                    var offsetDataRO = job.GetOffsetData();
                    
                    // Compute new position and update
                    pos += SystemAPI.Time.DeltaTime * lThreat.ValueRO.speed * lThreat.ValueRO.direction;
                    pos.y = threatTransform.ValueRO.Position.y + offsetDataRO.value.y;
                    
                    lTransform.ValueRW.Position = pos;

                    job.Dispose();
                }
         
            }
        }

    }
}