using Levels.Components;
using UnityEngine;
using Unity.Entities;
using Player.Components;

namespace Levels.Authorings
{
    public class TileAuthoring : MonoBehaviour
    {
        public LevelTiles tileType;
        
        private class Baker : Baker<TileAuthoring>
        {
            public override void Bake(TileAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new Tile()
                {
                    tileType = (byte) authoring.tileType
                });
            }
        }

        
    }
}
