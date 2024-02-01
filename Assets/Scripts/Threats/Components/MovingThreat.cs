using Unity.Entities;
using Unity.Mathematics;

namespace Threats.Components
{
    public struct MovingThreat : IComponentData
    {
        public float speed;
        public float3 direction;
    }
}