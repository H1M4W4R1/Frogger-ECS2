using Levels.Components;
using Player.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

namespace Levels.Systems.Jobs
{
    [BurstCompile]
    public partial struct ResetPlayerPositionToStart : IJobEntity, INativeDisposable
    {
        private NativeArray<float3> _startingPosition;

        [BurstCompile]
        public static void Prepare(out ResetPlayerPositionToStart job, in LevelData data)
        {
            Prepare(out job, data.startingPosition);
        }
        
        [BurstCompile] 
        public static void Prepare(out ResetPlayerPositionToStart job, in float3 startPosition)
        {
            job = new ResetPlayerPositionToStart()
            {
                _startingPosition = new NativeArray<float3>(1, Allocator.TempJob)
                {
                    [0] = startPosition
                }
            };
        }
        
        [BurstCompile]
        public void Execute(in PlayerTag playerTag, ref LocalTransform localTransform)
        {
            localTransform.Position = _startingPosition[0];
        }

        public void Dispose()
        {
            _startingPosition.Dispose();
        }

        public JobHandle Dispose(JobHandle inputDeps)
        {
            return _startingPosition.Dispose(inputDeps);
        }
    }
}