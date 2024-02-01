using Levels.Components;
using Player.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct AttemptJumpJob : IJobEntity
    {
        public byte foundTileId;

        [BurstCompile]
        public static void Prepare(out AttemptJumpJob job, byte tileId)
        {
            job = new AttemptJumpJob()
            {
                foundTileId = tileId
            };
        }
        
        [BurstCompile]
        public void Execute(in LocalTransform localTransform, ref CurrentMovementData movementData,
            in PlayerInput input, in PlayerMovementSettings movementSettings)
        {
            // Tile is null
            if (foundTileId == (byte) LevelTiles.None)
            {
                
                return;
            }
            if (movementData.movementVectorNonNormalized is {x: 0, z: 0}) return;
            
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