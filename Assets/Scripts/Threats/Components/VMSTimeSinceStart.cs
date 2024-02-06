using Unity.Entities;

namespace Threats.Components
{
    public struct VMSTimeSinceStart : IComponentData
    {
        public float timePassed;
    }
}