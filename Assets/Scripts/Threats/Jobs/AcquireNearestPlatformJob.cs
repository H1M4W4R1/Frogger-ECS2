using System;
using Threats.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Threats.Jobs
{
    [BurstCompile]
    public partial struct AcquireNearestPlatformJob : IJobEntity, INativeDisposable
    {
        public NativeArray<MovingThreat> nearestPlatformInfo;
        public NativeArray<float> nearestDistanceSquared;
        
        public float3 forPosition;

        [BurstCompile]
        public bool IsPlatformNull()
        {
            return !nearestPlatformInfo[0].isNotNull;
        }

        [BurstCompile]
        public float3 GetPosition()
        {
            return nearestPlatformInfo[0].currentPosition;
        }
        
        [BurstCompile]
        public void Execute(in LocalTransform localTransform, ref MovingThreat threatObj)
        {
            // Compute squared distance
            var platformPos = localTransform.Position;
            var deltaDistance = platformPos - forPosition;
            var sqrDistance = deltaDistance.x * deltaDistance.x + deltaDistance.z * deltaDistance.z;

            // Check if platform is closer to target position and assign reference to platform at array
            if (nearestDistanceSquared[0] > sqrDistance)
            {
                nearestDistanceSquared[0] = sqrDistance;
                nearestPlatformInfo[0] = threatObj;
            }
        }
 
        [BurstCompile]
        public static void Prepare(out AcquireNearestPlatformJob job, in float3 forPosition)
        {
            job = new AcquireNearestPlatformJob()
            {
                nearestPlatformInfo = new NativeArray<MovingThreat>(1, Allocator.Domain),
                nearestDistanceSquared = new NativeArray<float>(1, Allocator.Domain)
                {
                    [0] = 1e3f
                },
                forPosition = forPosition
            };
        }

        public void Dispose()
        {
            nearestDistanceSquared.Dispose();
            nearestPlatformInfo.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            inputDeps = nearestDistanceSquared.Dispose(inputDeps);
            inputDeps = nearestPlatformInfo.Dispose(inputDeps);

            return inputDeps;
        }

    }
}