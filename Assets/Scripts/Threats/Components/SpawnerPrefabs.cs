using Unity.Entities;

namespace Threats.Components
{
    public struct SpawnerPrefabs : IBufferElementData
    {
        public Entity prefab;
    }
}