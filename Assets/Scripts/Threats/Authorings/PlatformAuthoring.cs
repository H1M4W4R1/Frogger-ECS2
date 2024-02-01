using UnityEngine;
using Unity.Entities;
using Player.Components;
using Threats.Components;
using Unity.Mathematics;

namespace Threats.Authorings
{
    public class PlatformAuthoring : MonoBehaviour
    {
        public float speed;
        public float3 direction = new float3(-1, 0, 0);
        
        private class Baker : Baker<PlatformAuthoring>
        {
            public override void Bake(PlatformAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent<IsPlatform>(e);
                
                AddComponent(e, new MovingThreat()
                {
                    speed = authoring.speed,
                    direction = authoring.direction,
                    isNotNull = true
                });
            }
        }

        
    }
}
