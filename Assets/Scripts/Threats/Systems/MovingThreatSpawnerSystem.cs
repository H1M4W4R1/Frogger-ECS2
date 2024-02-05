using Management.Components;
using Threats.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Threats.Systems
{
    public partial struct MovingThreatSpawnerSystem : ISystem
    {
        private Random _rng;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _rng = new Random(1);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (spawner, timer, sEntity) in SystemAPI.Query<RefRO<MovingThreatSpawner>, RefRW<TimeUntilSpawn>>().WithEntityAccess())
            {
                var spawnerTransform = SystemAPI.GetComponent<LocalTransform>(sEntity);
                
                var prefabs = SystemAPI.GetBuffer<SpawnerPrefabs>(sEntity);
                
                // Update time
                timer.ValueRW.time -= SystemAPI.Time.DeltaTime;
                if (timer.ValueRO.time > 0f) continue;
                timer.ValueRW.time = _rng.NextFloat(spawner.ValueRO.minDelay, spawner.ValueRO.maxDelay);
                    
                // Spawn
                var prefab = prefabs[_rng.NextInt(0, prefabs.Length)].prefab;
                var rotation = SystemAPI.GetComponent<LocalTransform>(prefab).Rotation;
                var entityObject = ecb.Instantiate(prefab);
                    
                // Update position and rotation
                var lt = new LocalTransform
                {
                    Position = spawnerTransform.Position,
                    Rotation = rotation,
                    Scale = 1f
                };
                ecb.SetComponent(entityObject, lt);
                ecb.AddComponent(entityObject, new Lifetime(){time = spawner.ValueRO.lifetime});
            }

            // Create objects
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

    }
}