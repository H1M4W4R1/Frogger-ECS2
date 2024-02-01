using Levels.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Player.Systems.Jobs
{

    [BurstCompile]
    public partial struct AcquireTileAtPositionJob : IJobEntity
    {
        public NativeArray<byte> foundTileType;

        public int x;
        public int z;

        [BurstCompile]
        public void Execute(in RenderedLevelTile tile)
        {
            
            if (tile.xPosition == x && tile.zPosition == z)
                foundTileType[0] = tile.tileId;
        }
    }
}