using System;
using Helpers;
using Levels.Components;
using Player.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct AttemptJumpJob : IJobEntity
    {
        public byte foundTileId;
        public byte isPlatformJump;

        [BurstCompile]
        public static void Prepare(out AttemptJumpJob job, byte tileId, byte isPlatformJump)
        {
            job = new AttemptJumpJob()
            {
                foundTileId = tileId,
                isPlatformJump = isPlatformJump
            };
        }
        
        [BurstCompile]
        public void Execute(in LocalTransform localTransform, ref CurrentMovementData movementData,
            in PlayerInput input, in PlayerMovementSettings movementSettings)
        {
            // Tile is null or vector does not indicate movement
            if (foundTileId == (byte) LevelTiles.None) return;
            if (movementData.movementVectorNonNormalized is {x: 0, z: 0}) return;

            // Fixes dual-jump issue before vector is cleared
            if (Math.Abs(movementData.startingPosition.x - movementData.lastJumpTarget.x) < 0.1f &&
                Math.Abs(movementData.startingPosition.z - movementData.lastJumpTarget.z) < 0.1f) return;

            // Fix tile jump normalization
            if (isPlatformJump == 0)
            {
                movementData.lastJumpTarget = new float3(
                    MathHelper.TileInt(movementData.lastJumpTarget.x),
                    MathHelper.TileInt(movementData.lastJumpTarget.y),
                    MathHelper.TileInt(movementData.lastJumpTarget.z)
                );
            }

            // TODO: Acquire nearest tile type, if is water / kill tile then acquire nearest platform
            // TODO: If acquired object is platform then compute points for next meetTick, take jump time into account
            // TODO: If platform points in next meetTick are OK then assume platform location and jump onto platform
            // TODO: If platform moves out of your scope, then DIE
            // TODO: If platform dives then DIE
            // TODO: If touches world edge, then wait for push-off and DIE
            // TODO: If not water/kill tile then gently jump to next tile
            
            movementData.isMovementRequested = true;
        }
    }
}