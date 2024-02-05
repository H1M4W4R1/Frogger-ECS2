using Unity.Entities;

namespace Threats.Components
{
    public struct MovingThreatSpawner : IComponentData
    {
        public float minDelay;
        public float maxDelay;
        public float lifetime;

        public MovingThreatSpawner(float minDelay, float maxDelay, float lifetime)
        {
            this.minDelay = minDelay;
            this.maxDelay = maxDelay;
            this.lifetime = lifetime;
        }
    }
}