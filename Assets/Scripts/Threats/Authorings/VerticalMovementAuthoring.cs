using UnityEngine;
using Unity.Entities;
using Player.Components;
using Threats.Components;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Threats.Authorings
{
    public class VerticalMovementAuthoring : MonoBehaviour
    {
        public float minHeight = -1.5f;
        public float maxHeight = -0.5f;

        public float underwaterHeight = -1f;
        
        public float minCycleTime = 2f;
        public float maxCycleTime = 5f;

        public int sinePower = 10;
        
        private class Baker : Baker<VerticalMovementAuthoring>
        {
            public override void Bake(VerticalMovementAuthoring authoring)
            {
                var rng = new Random(125);
                
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new VerticalSineMovement()
                {
                    minHeight = authoring.minHeight,
                    maxHeight = authoring.maxHeight,
                    underwaterHeight = authoring.underwaterHeight,
                    cycleTime = rng.NextFloat(authoring.minCycleTime, authoring.maxCycleTime),
                    sinePower = rng.NextInt(5, 25)
                });
                AddComponent<VMSTimeSinceStart>(e);
                
                AddComponent<IsPlatformUnderwater>(e);
                SetComponentEnabled<IsPlatformUnderwater>(e, false);
            }
        }

        
    }
}
