using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class LevelDataAuthoring : MonoBehaviour
    {
        public float timeUntilFailed;
        public float threatSpeedMultiplier;
        public int halfPlayableWidth = 3;
        public int halfRenderedWidth = 16;
        
        private class Baker : Baker<LevelDataAuthoring>
        {
            public override void Bake(LevelDataAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelData()
                {
                   time = authoring.timeUntilFailed,
                   threatSpeed = authoring.threatSpeedMultiplier,
                   levelHalfPlayableWidth = authoring.halfPlayableWidth,
                   levelHalfRenderedWidth = authoring.halfRenderedWidth
                });
            }
        }

        
    }
}
