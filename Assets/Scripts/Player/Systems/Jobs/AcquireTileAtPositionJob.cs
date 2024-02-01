using System;
using Helpers;
using Levels.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Player.Systems.Jobs
{

    [BurstCompile]
    public partial struct AcquireTileAtPositionJob : IJobEntity, INativeDisposable
    {
        public NativeArray<byte> foundTileType;

        public int x;
        public int z;

        [BurstCompile]
        public byte GetFoundTileType() => foundTileType[0];
        
        [BurstCompile] 
        public void Execute(in RenderedLevelTile tile)
        {
            if (tile.xPosition == x && tile.zPosition == z)
                foundTileType[0] = tile.tileId;
        }

        [BurstCompile]
        public static void Prepare(out AcquireTileAtPositionJob job, float xValue, float zValue)
        {
            Prepare(out job, MathHelper.TileInt(xValue), MathHelper.TileInt(zValue));
        }
        
        [BurstCompile]
        public static void Prepare(out AcquireTileAtPositionJob job, int xValue, int zValue)
        {
            job = new AcquireTileAtPositionJob()
            {
                x = xValue,
                z = zValue,
                foundTileType = new NativeArray<byte>(1, Allocator.TempJob)
            };
        }
        
        public void Dispose()
        {
            foundTileType.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return foundTileType.Dispose(inputDeps);
        }
    }
}