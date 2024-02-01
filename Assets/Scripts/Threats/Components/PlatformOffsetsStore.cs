using Unity.Entities;
using Unity.Mathematics;

namespace Threats.Components
{
    public struct PlatformOffsetsStore : IBufferElementData
    {
        public float3 value;
    }
}