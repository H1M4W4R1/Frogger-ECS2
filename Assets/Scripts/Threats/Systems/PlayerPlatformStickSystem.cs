using Player.Components;
using Threats.Components;
using Unity.Burst;
using Unity.Entities;
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
                             
                    // Get nearest offset
                    var nOffsetDistanceSquare = 10e3f;
                    var offsetDataRO = default(PlatformOffsetsStore);

                    foreach (var offset in offsets)
                    {
                        // Compute difference between platform offset and local position of player
                        var diff = threatTransform.ValueRO.Position + offset.value - pos;
                        var dist = diff.x * diff.x + diff.z * diff.z;

                        if (dist < nOffsetDistanceSquare)
                        {
                            nOffsetDistanceSquare = dist;
                            offsetDataRO = offset;
                        }
                    }
                    
                    // Compute new position and update
                    pos += SystemAPI.Time.DeltaTime * lThreat.ValueRO.speed * lThreat.ValueRO.direction;
                    pos.y = threatTransform.ValueRO.Position.y + offsetDataRO.value.y;
                    
                    lTransform.ValueRW.Position = pos;
                }
         
            }
        }

    }
}