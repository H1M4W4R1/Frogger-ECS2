using System;
using System.Collections.Generic;
using System.Linq;
using Levels.Components;
using UnityEngine;
using Unity.Entities;

namespace Levels.Authorings
{
    public class TileDataAuthoring : MonoBehaviour
    {

        public List<TileAuthoring> tiles = new List<TileAuthoring>();

        private class Baker : Baker<TileDataAuthoring>
        {
            public override void Bake(TileDataAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                var buffer = AddBuffer<TileLibrary>(e);

                var nCheck = 0;
                
                // Create tile information buffer
                foreach (var tile in authoring.tiles.OrderBy(q => q.tileType))
                {
                    var tileId = (byte) tile.tileType;
                    
                    var tData = new TileLibrary
                    {
                        tile = GetEntity(tile, TransformUsageFlags.Dynamic),
                        index = tileId
                    };

                    if (tileId != nCheck)
                        throw new Exception($"Failed: missing tile with id {tileId}");

                    nCheck++;

                    buffer.Add(tData);
                }
            }
        }

        
    }
}
