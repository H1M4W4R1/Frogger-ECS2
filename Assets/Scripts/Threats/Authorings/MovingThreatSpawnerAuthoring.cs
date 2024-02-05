using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Threats.Components;
using Unity.Transforms;

namespace Threats.Authorings
{
    public class MovingThreatSpawnerAuthoring : MonoBehaviour
    {

        public float minDelay;
        public float maxDelay;

        public float lifetime;

        public List<GameObject> prefabs = new List<GameObject>();

        private class Baker : Baker<MovingThreatSpawnerAuthoring>
        {
            public override void Bake(MovingThreatSpawnerAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
         
                AddComponent(e, new MovingThreatSpawner(authoring.minDelay, authoring.maxDelay, authoring.lifetime));
                AddComponent(e, new TimeUntilSpawn() {time = 0f});
                
                
                var dBuffer = AddBuffer<SpawnerPrefabs>(e);
                
                // Register prefabs
                foreach (var q in authoring.prefabs)
                {
                    dBuffer.Add(new SpawnerPrefabs()
                    {
                        prefab = GetEntity(q, TransformUsageFlags.Dynamic)
                    });
                }

            }
        }

        
    }
}
