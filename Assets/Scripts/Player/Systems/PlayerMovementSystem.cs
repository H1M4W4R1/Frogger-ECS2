using Audio.Components;
using Levels.Components;
using LowLevel;
using Player.Aspects;
using Player.Components;
using Player.Systems.Enums;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace Player.Systems
{
    public partial struct PlayerMovementSystem : ISystem
    {
       
        private bool _isMoving;
        private float _jumpTimer;
        private float _jumpTotalTime;
        private float3 _jumpVector;
        private float3 _startingPosition;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerMovement>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Grab level width
            var onlySingleLevelFound = SystemAPI.TryGetSingleton(out LevelData levelData);
            var sfxDataSingletonFound = 
                SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<SFXData> sfxData);
            
            // Process movement
            foreach(PlayerAspect aspect in SystemAPI.Query<PlayerAspect>())
            {
                // Decode aspect information
                var localTransform = aspect.localTransform;
                var input = aspect.input;
                var movement = aspect.movement;
                var movementRO = aspect.movement.ValueRO;
                var jumpSFX = aspect.jumpSFX.ValueRO;

                // Automatically update level information
                if (onlySingleLevelFound)
                    movement.ValueRW.maxTilesToSide = levelData.levelHalfPlayableWidth;

                if (!_isMoving)
                {
                    // Initalize jump vector
                    _jumpVector.x = _jumpVector.y = _jumpVector.z = 0;

                    // Compute starting position for further usage
                    _startingPosition = localTransform.ValueRO.Position;
                    JumpDirection jumpDirection = JumpDirection.None;

                    // Horizontal jumps
                    if (input.ValueRO.axisInput.x < -ConstConfig.AXIS_DEADZONE)
                    {
                        if (_startingPosition.x > -movementRO.maxTilesToSide * ConstConfig.TILE_SIZE)
                            _jumpVector.x -= movementRO.jumpDistance;
                        jumpDirection = JumpDirection.Left;
                    }
                    else if (input.ValueRO.axisInput.x > ConstConfig.AXIS_DEADZONE)
                    {
                        if (_startingPosition.x < movementRO.maxTilesToSide * ConstConfig.TILE_SIZE)
                            _jumpVector.x += movementRO.jumpDistance;
                        jumpDirection = JumpDirection.Right;
                    }
                    // Vertical jumps
                    else if (input.ValueRO.axisInput.y < -ConstConfig.AXIS_DEADZONE)
                    {
                        if (_startingPosition.z > ConstConfig.BACKWARD_LIMIT)
                            _jumpVector.z -= movementRO.jumpDistance;
                        jumpDirection = JumpDirection.Down;
                    }
                    else if (input.ValueRO.axisInput.y > ConstConfig.AXIS_DEADZONE)
                    {
                        _jumpVector.z += movementRO.jumpDistance;
                        jumpDirection = JumpDirection.Up;
                    }
                    else
                        return;

                    _isMoving = true;
                    _jumpTimer = _jumpTotalTime = movementRO.jumpDistance / movementRO.jumpSpeed;

                    // Register jump SFX
                    if (sfxDataSingletonFound)
                    {
                        sfxData.Add(new SFXData()
                        {
                            sfxId = jumpSFX.sfxId
                        });
                    }

                    // Handle player rotation during jump (rotation is -90, 90, 0 and 180deg in radians)
                    if (movementRO.rotatePlayerCharacter)
                    {
                        switch (jumpDirection)
                        {
                            case JumpDirection.Left:
                                localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.LEFT_ROTATION, 0));
                                break;
                            case JumpDirection.Right:
                                localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.RIGHT_ROTATION, 0));
                                break;
                            case JumpDirection.Up:
                                localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.UP_ROTATION, 0));
                                break;
                            case JumpDirection.Down:
                                localTransform.ValueRW.Rotation = quaternion.EulerXYZ(new float3(0, ConstConfig.DOWN_ROTATION, 0));
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
}
