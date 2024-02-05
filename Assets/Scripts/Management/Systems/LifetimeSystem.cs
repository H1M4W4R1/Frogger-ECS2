using Management.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace Management.Systems
{
    public partial struct LifetimeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Perform lifetime cycle
            var qDestroy = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (lt, entity) in SystemAPI.Query<RefRW<Lifetime>>().WithEntityAccess())
            {
                lt.ValueRW.time -= SystemAPI.Time.DeltaTime;
                
                if (lt.ValueRO.time <= 0)
                {
                    qDestroy.DestroyEntity(entity);
                }
            }
            
            qDestroy.Playback(state.EntityManager);
            qDestroy.Dispose();
        }

    }
}