using Player.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct AnimateFrogJumpJob : IJobEntity
    {
        public float jumpTimer;

        [BurstCompile]
        public static void Prepare(out AnimateFrogJumpJob job, float cTime)
        {
            job = new AnimateFrogJumpJob()
            {
                jumpTimer = cTime
            };
        }
        
        [BurstCompile]
        public void Execute(ref LocalTransform localTransform, in PlayerMovementSettings movementSettings,
            in CurrentMovementData movementData)
        {
            var xPercentage = (1f - jumpTimer / (movementSettings.jumpDistance / movementSettings.jumpSpeed));

            // X position (percentage of completion multiplied by jump vector)
            var posDelta = xPercentage * movementData.movementVectorNonNormalized;

            // Y position Y = -(2X-1)^2 - 1, where X is normalized in [0, 1] range
            var quadrant = (2 * xPercentage - 1);
            var yPosition = -(quadrant * quadrant) + 1 * movementSettings.jumpHeight;
            posDelta.y = yPosition > 0 ? yPosition : 0; // Cannot exceed 0f (ground level)

            localTransform.Position = movementData.startingPosition + posDelta;
        }
    }
}