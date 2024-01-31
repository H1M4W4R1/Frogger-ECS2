using LowLevel;
using Player.Components;
using Player.Systems.Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct CheckForJumpAttemptJob : IJobEntity
    {
        [BurstCompile]
        public void Execute(in LocalTransform localTransform, ref CurrentMovementData movementData,
            in PlayerInput input, in PlayerMovementSettings movementSettings)
        {
            movementData.isComputing = true;
            
            // Initalize jump vector
            movementData.movementVectorNonNormalized.x = 0;
            movementData.movementVectorNonNormalized.y = 0;
            movementData.movementVectorNonNormalized.z = 0;

            // Compute starting position for further usage
            movementData.startingPosition = localTransform.Position;
            movementData.direction = JumpDirection.None;

            // Horizontal jumps
            if (input.axisInput.x < -ConstConfig.AXIS_DEADZONE)
            {
                if (movementData.startingPosition.x > -movementSettings.maxTilesToSide * ConstConfig.TILE_SIZE)
                    movementData.movementVectorNonNormalized.x -= movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Left;
                movementData.isMoving = true;
            }
            else if (input.axisInput.x > ConstConfig.AXIS_DEADZONE)
            {
                if (movementData.startingPosition.x < movementSettings.maxTilesToSide * ConstConfig.TILE_SIZE)
                    movementData.movementVectorNonNormalized.x += movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Right;
                movementData.isMoving = true;
            }
            // Vertical jumps
            else if (input.axisInput.y < -ConstConfig.AXIS_DEADZONE)
            {
                if (movementData.startingPosition.z > ConstConfig.BACKWARD_LIMIT)
                    movementData.movementVectorNonNormalized.z -= movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Down;
                movementData.isMoving = true;
            }
            else if (input.axisInput.y > ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.z += movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Up;
                movementData.isMoving = true;
            }

           
            movementData.isComputing = false;
        }
    }
}