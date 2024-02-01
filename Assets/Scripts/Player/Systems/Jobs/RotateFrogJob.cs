using LowLevel;
using Player.Components;
using Player.Systems.Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Player.Systems.Jobs
{
    [BurstCompile]
    public partial struct RotateFrogJob : IJobEntity
    {
        [BurstCompile]
        public static void Prepare(out RotateFrogJob job)
        {
            job = new RotateFrogJob();
        }
        
        [BurstCompile]
        public void Execute(in PlayerTag tag, ref LocalTransform localTransform, in CurrentMovementData currentMovementData,
            in PlayerMovementSettings movementSettingsSettings)
        {
            if (!movementSettingsSettings.rotatePlayerCharacter) return;
            
            switch (currentMovementData.direction)
            {
                case JumpDirection.Left:
                    localTransform.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.LEFT_ROTATION, 0));
                    break;
                case JumpDirection.Right:
                    localTransform.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.RIGHT_ROTATION, 0));
                    break;
                case JumpDirection.Up:
                    localTransform.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.UP_ROTATION, 0));
                    break;
                case JumpDirection.Down:
                    localTransform.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.DOWN_ROTATION, 0));
                    break;
            }
        }
    }
}