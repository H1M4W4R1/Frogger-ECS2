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
            state.RequireForUpdate<LocalTransform>();
            state.RequireForUpdate<MovingThreat>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (localTransform, threat) 
                     in SystemAPI.Query<RefRW<LocalTransform>, RefRO<MovingThreat>>())
            {
                var tData = threat.ValueRO;
                var vData = localTransform.ValueRW.Position + tData.direction * tData.speed * SystemAPI.Time.DeltaTime;
                localTransform.ValueRW.Position = vData;
            }

            /*var job = AcquireNearestPlatformJob.Prepare();
            job.Schedule(state.Dependency).Complete();
            
            // Dispose useless array
            state.Dependency = job.Dispose(state.Dependency);*/
        }
    }
}