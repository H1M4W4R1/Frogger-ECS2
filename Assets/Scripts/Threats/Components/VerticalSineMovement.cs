using Unity.Entities;

namespace Threats.Components
{
    public struct VerticalSineMovement : IComponentData
    {
        public float maxHeight;
        public float minHeight;

        public float underwaterHeight;
        
        public float cycleTime;

        public int sinePower;
    }
}