using Levels.Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Levels.Authorings
{
    public class LevelDataAuthoring : MonoBehaviour
    {
        public float timeUntilFailed;
        public int halfPlayableWidth = 3;
        public int halfRenderedWidth = 16;
        public TileAuthoring startingTile;
        
        private class Baker : Baker<LevelDataAuthoring>
        {
            public override void Bake(LevelDataAuthoring authoring)
            {
                float3 startPos = float3.zero;
                if (authoring.startingTile)
                {
                    startPos = authoring.startingTile.transform.position;
                    startPos.y = 0;
                }

                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelData()
                {
                   time = authoring.timeUntilFailed,
                   levelHalfPlayableWidth = authoring.halfPlayableWidth,
                   levelHalfRenderedWidth = authoring.halfRenderedWidth,
                   startingPosition =  startPos
                });
            }
        }

        
    }
}
