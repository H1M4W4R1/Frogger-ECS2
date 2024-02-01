using Levels.Components;
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
    public partial struct ComputeJumpVectorJob : IJobEntity
    {
        [BurstCompile]
        public static void Prepare(out ComputeJumpVectorJob job)
        {
            job = new ComputeJumpVectorJob();
        }
        
        [BurstCompile]
        public void Execute(in LocalTransform localTransform, ref CurrentMovementData movementData,
            in PlayerInput input, in PlayerMovementSettings movementSettings)
        {

            movementData.isMovementComputing = true;

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
                movementData.movementVectorNonNormalized.x = -movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Left;
            }
            else if (input.axisInput.x > ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.x = movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Right;
            }
            // Vertical jumps
            else if (input.axisInput.y < -ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.z = -movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Down;
            }
            else if (input.axisInput.y > ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.z = movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Up;
            }

            movementData.isMovementComputing = false;
        }
    }
}