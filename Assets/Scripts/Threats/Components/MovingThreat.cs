using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Threats.Components
{
    public struct MovingThreat : IComponentData
    {
        public bool isNotNull;
        public float speed;
        public float3 direction;

        public float3 currentPosition;
    }
}