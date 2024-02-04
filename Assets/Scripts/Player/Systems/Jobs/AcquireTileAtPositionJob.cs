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
        public NativeArray<Tile> foundTile;

        public int x;
        public int z;

        [BurstCompile]
        public byte GetFoundTileType() => foundTile[0].tileId;

        [BurstCompile]
        public bool IsKillTile() => foundTile[0].isKillTile;
        
        [BurstCompile]
        public bool IsKillZone() => foundTile[0].isKillZone;
        
        [BurstCompile]
        public void Execute(in Tile tile)
        {
            if (tile.xPosition == x && tile.zPosition == z)
                foundTile[0] = tile;
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
                foundTile = new NativeArray<Tile>(1, Allocator.TempJob)
            };
        }
        
        public void Dispose()
        {
            foundTile.Dispose();
        }

        [BurstCompile]
        public JobHandle Dispose(JobHandle inputDeps)
        {
            return foundTile.Dispose(inputDeps);
        }
    }
}