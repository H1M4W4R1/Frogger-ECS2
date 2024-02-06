using Threats.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Threats.Systems
{
    public partial struct VerticalSineMovementSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (vms, lTransform, timer, e) in 
                     SystemAPI.Query<RefRO<VerticalSineMovement>, RefRW<LocalTransform>, RefRW<VMSTimeSinceStart>>()
                         .WithEntityAccess())
            {
                // Move timer
                timer.ValueRW.timePassed += Time.deltaTime;
                if (timer.ValueRO.timePassed > vms.ValueRO.cycleTime)
                    timer.ValueRW.timePassed -= vms.ValueRO.cycleTime;

                // Compute new position
                var pos = lTransform.ValueRO.Position;
                var t = timer.ValueRO.timePassed / vms.ValueRO.cycleTime * math.PI * 2; // 2pi * [0,1]
                var sin = 2 * (1 - math.pow(math.abs(math.sin(t)), vms.ValueRO.sinePower) - 0.5f);
                
                // Update data
                pos.y = vms.ValueRO.minHeight + sin * (vms.ValueRO.maxHeight - vms.ValueRO.minHeight);
                SystemAPI.SetComponentEnabled<IsPlatformUnderwater>(e, pos.y < vms.ValueRO.underwaterHeight);
                lTransform.ValueRW.Position = pos;
            }
        }

    }
}