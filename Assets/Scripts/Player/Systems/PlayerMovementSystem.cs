using Assets.Scripts.Player.Components;
using Assets.Scripts.Player.Systems.Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial struct PlayerMovementSystem : ISystem
{
    private const float AXIS_DEADZONE = 0.2f;
    private bool _isMoving;
    private float _jumpTimer;
    private float _jumpTotalTime;
    private float3 _jumpVector;
    private float3 _startingPosition;

    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerMovement>();
        state.RequireForUpdate<PlayerInput>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((RefRW<LocalTransform> localTransform, RefRO<PlayerMovement> movement, RefRO<PlayerInput> input) in
            SystemAPI.Query<RefRW<LocalTransform>, RefRO<PlayerMovement>, RefRO<PlayerInput>>())
        {
            var movementRO = movement.ValueRO;

            if (!_isMoving)
            {
                // Initalize jump vector
                _jumpVector.x = _jumpVector.y = _jumpVector.z = 0;

                // Compute starting position for further usage
                _startingPosition = localTransform.ValueRO.Position;
                JumpDirection jumpDirection;

                // Horizontal jumps
                if (input.ValueRO.axisInput.x < -AXIS_DEADZONE)
                {
                    _jumpVector.x -= movementRO.jumpDistance;
                    jumpDirection = JumpDirection.Left;
                }
                else if (input.ValueRO.axisInput.x > AXIS_DEADZONE)
                {
                    _jumpVector.x += movementRO.jumpDistance;
                    jumpDirection = JumpDirection.Right;
                }
                // Vertical jumps
                else if (input.ValueRO.axisInput.y < -AXIS_DEADZONE)
                {
                    _jumpVector.z -= movementRO.jumpDistance;
                    jumpDirection = JumpDirection.Down;
                }
                else if (input.ValueRO.axisInput.y > AXIS_DEADZONE)
                {
                    _jumpVector.z += movementRO.jumpDistance;
                    jumpDirection = JumpDirection.Up;
                }
                else
                    return;

                _isMoving = true;
                _jumpTimer = _jumpTotalTime = movementRO.jumpDistance / movementRO.jumpSpeed;

                // Handle player rotation during jump
                if (movementRO.rotatePlayerCharacter)
                {
                    switch (jumpDirection)
                    {
                        case JumpDirection.Left:
                            localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, -1.57079633f, 0));
                            break;
                        case JumpDirection.Right:
                            localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, 1.57079633f, 0));
                            break;
                        case JumpDirection.Up:
                            localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, 0, 0));
                            break;
                        case JumpDirection.Down:
                            localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, 3.14159265f, 0));
                            break;
                    }
                }
            }
            else
            {
                // Animate jump
                var dt = SystemAPI.Time.DeltaTime;
                var xPercentage = (1f - _jumpTimer / _jumpTotalTime);

                // X position (percentage of completion multiplied by jump vector)
                var posDelta = xPercentage * _jumpVector;

                // Y position Y = -(2X-1)^2 - 1, where X is normalized in [0, 1] range
                var quadrant = (2 * xPercentage - 1);
                var yPosition = -(quadrant * quadrant) + 1 * movement.ValueRO.jumpHeight;
                posDelta.y = yPosition > 0 ? yPosition : 0; // Cannot exceed 0f (ground level)

                _jumpTimer -= dt;

                localTransform.ValueRW.Position = _startingPosition + posDelta;

                if (_jumpTimer <= 0f)
                {
                    // BUGHUNTER: when timer lags under 0f position will be terribly calculated, fix that using precalculated target position
                    localTransform.ValueRW.Position = _startingPosition + _jumpVector;

                   _isMoving = false;
                }
            }

        }      
    }


}
