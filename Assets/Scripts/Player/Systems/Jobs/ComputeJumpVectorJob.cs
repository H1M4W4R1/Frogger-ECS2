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

            // TODO: Acquire nearest tile, if is water / kill tile then acquire nearest platform
            // TODO: If acquired object is platform then compute points for next meetTick, take jump time into account
            // TODO: If platform points in next meetTick are OK then assume platform location and jump onto platform
            // TODO: If platform moves out of your scope, then DIE
            // TODO: If platform dives then DIE
            // TODO: If touches world edge, then wait for push-off and DIE
            // TODO: If not water/kill tile then gently jump to next tile

            // Horizontal jumps
            if (input.axisInput.x < -ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.x -= movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Left;
            }
            else if (input.axisInput.x > ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.x += movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Right;
            }
            // Vertical jumps
            else if (input.axisInput.y < -ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.z -= movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Down;
            }
            else if (input.axisInput.y > ConstConfig.AXIS_DEADZONE)
            {
                movementData.movementVectorNonNormalized.z += movementSettings.jumpDistance;
                movementData.direction = JumpDirection.Up;
            }

            movementData.isComputing = false;
        }
    }
}