using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class TileAuthoring : MonoBehaviour
    {
        public LevelTiles tileType;
        public bool isKillTile;
        public bool isKillZone;
        
        private class Baker : Baker<TileAuthoring>
        {
            public override void Bake(TileAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new Tile
                {
                    tileId = 255,
                    isKillTile = authoring.isKillTile,
                    isKillZone = authoring.isKillZone
                });
                
                // Tags
                AddComponent<IsKillTile>(e);
                AddComponent<IsKillZone>(e);
                
                SetComponentEnabled<IsKillTile>(e, authoring.isKillTile);
                SetComponentEnabled<IsKillZone>(e, authoring.isKillZone);
            }
        }

        
    }
}
