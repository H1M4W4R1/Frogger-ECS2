using Threats.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct AcquireNearestPlatformOffset : IJob, INativeDisposable
    {
        private NativeArray<PlatformOffsetsStore> _nearestOffset;

        private float3 _offsetReferencePosition;
        private float3 _forPosition;
        private DynamicBuffer<PlatformOffsetsStore> _buffer;

        [BurstCompile]
        public static void Prepare(out AcquireNearestPlatformOffset job, in DynamicBuffer<PlatformOffsetsStore> offsets, 
            in float3 forPosition, in float3 refPosition)
        {
            job = new AcquireNearestPlatformOffset()
            {
                _forPosition = forPosition,
                _buffer = offsets,
                _offsetReferencePosition = refPosition,
                _nearestOffset = new NativeArray<PlatformOffsetsStore>(1, Allocator.TempJob)
            };
        }
        
        [BurstCompile]
        public void Execute()
        {
            // Get nearest offset
            var nOffsetDistanceSquare = 10e3f;
            var offsetDataRO = default(PlatformOffsetsStore);

            foreach (var offset in _buffer)
            {
                // Compute difference between platform offset and local position of player
                var diff = _offsetReferencePosition + offset.value - _forPosition;
                var dist = diff.x * diff.x + diff.z * diff.z;

                if (dist < nOffsetDistanceSquare)
                {
                    nOffsetDistanceSquare = dist;
                    offsetDataRO = offset;
                }
            }

            _nearestOffset[0] = offsetDataRO;
        }

        public void Dispose()
        {
            _nearestOffset.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return _nearestOffset.Dispose(inputDeps);
        }

        [BurstCompile]
        public PlatformOffsetsStore GetOffsetData()
        {
            return _nearestOffset[0];
        }
    }
}