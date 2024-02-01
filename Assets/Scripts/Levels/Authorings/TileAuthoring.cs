using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class TileAuthoring : MonoBehaviour
    {
        public LevelTiles tileType;
        public bool isKillTile;
        
        private class Baker : Baker<TileAuthoring>
        {
            public override void Bake(TileAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new Tile()
                {
                    tileType = (byte) authoring.tileType,
                    isKillTile = authoring.isKillTile
                });
                AddComponent(e, new RenderedLevelTile
                {
                    tileId = 255
                });
            }
        }

        
    }
}
