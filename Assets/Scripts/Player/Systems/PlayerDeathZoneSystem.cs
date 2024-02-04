using Player.Aspects;
using Player.Components;
using Player.Systems.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Player.Systems
{
    public partial struct PlayerDeathZoneSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Process movement
            foreach ((PlayerAspect aspect, Entity e) in SystemAPI.Query<PlayerAspect>().WithEntityAccess())
            {
                // Get position of player
                var lPos = aspect.localTransform.ValueRO.Position;
                
                // Check tiles
                AcquireTileAtPositionJob.Prepare(out var tileAccessJob, lPos.x, lPos.z);
                tileAccessJob.Schedule(state.Dependency).Complete();

                // If player is dead
                if (tileAccessJob.IsKillZone())
                    SystemAPI.SetComponentEnabled<IsDead>(e, true);

                // We don't like leaks
                tileAccessJob.Dispose();
            }
        }
    }
}