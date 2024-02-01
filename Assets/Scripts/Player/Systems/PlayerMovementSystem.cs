using Audio.Components;
using Audio.LowLevel;
using Levels.Components;
using Player.Aspects;
using Player.Components;
using Player.Systems.Jobs;
using Unity.Burst;
using Unity.Entities;

namespace Player.Systems
{
    public partial struct PlayerMovementSystem : ISystem
    {
        private float _jumpTimer;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerTag>();
            state.RequireForUpdate<PlayerMovementSettings>();
            state.RequireForUpdate<CurrentMovementData>();
            state.RequireForUpdate<SFXInfo>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Process movement
            foreach ((PlayerAspect aspect, Entity e) in SystemAPI.Query<PlayerAspect>().WithEntityAccess())
            {
                // Decode aspect information
                var movementRO = aspect.movement.ValueRO;
                var movementInfo = aspect.movementInformation.ValueRO;
                var lPos = aspect.localTransform.ValueRO.Position;

                // Update component information
                SystemAPI.SetComponentEnabled<HasMovementRequest>(e, movementInfo.isMovementRequested);
                SystemAPI.SetComponentEnabled<IsMovementComputing>(e, movementInfo.isMovementComputing);
                SystemAPI.SetComponentEnabled<IsStandingStill>(e, movementInfo is 
                    {isMovementRequested: false, isMovementComputing: false, isJumpAnimating: false});

                if (movementInfo.isMovementComputing) return;

                if (!movementInfo.isJumpAnimating)
                {
                    if (!movementInfo.isMovementRequested)
                    {
                        // Compute vector
                        ComputeJumpVectorJob.Prepare(out var cvJob);
                        cvJob.Schedule(state.Dependency).Complete();
                        
                        lPos += movementInfo.movementVectorNonNormalized;

                        // Skip empty vector
                        if (movementInfo.movementVectorNonNormalized is {x: 0, z: 0}) return;

                        // Check tiles
                        AcquireTileAtPositionJob.Prepare(out var tileAccessJob, lPos.x, lPos.z);
                        tileAccessJob.Schedule(state.Dependency).Complete();
                        
                        // Update tile data
                        var tileType = tileAccessJob.GetFoundTileType();
                        SystemAPI.SetComponentEnabled<IsTargetTileNull>(e, tileType == (byte) LevelTiles.None);
                        
                        // Attempt jumping onto tile
                        AttemptJumpJob.Prepare(out var attemptJump, tileType);
                        attemptJump.Schedule(state.Dependency).Complete();

                        // Free memory
                        state.Dependency = tileAccessJob.Dispose(state.Dependency);
                    }
                    else
                    {
                        _jumpTimer = movementRO.jumpDistance / movementRO.jumpSpeed;
                        aspect.movementInformation.ValueRW.isJumpAnimating = true;

                        // Handle player rotation during jump
                        RotateFrogJob.Prepare(out var rotateFrogJob);
                        state.Dependency = rotateFrogJob.Schedule(state.Dependency);

                        // Play SFX
                        if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<SFXInfo> info))
                            info.Add(new SFXInfo() {sfxClip = UniqueAudioClip.JumpSFX});
                    }
                }
                else
                {
                    // Animate jump 
                    var dt = SystemAPI.Time.DeltaTime;

                    // Jump animation
                    AnimateFrogJumpJob.Prepare(out var job, _jumpTimer);
                    job.Run();

                    _jumpTimer -= dt;

                    if (_jumpTimer <= 0f)
                    {
                        // BUGFIX: when timer lags under 0f position will be terribly calculated, fix that using precalculated target position
                        aspect.localTransform.ValueRW.Position =
                            movementInfo.startingPosition + movementInfo.movementVectorNonNormalized;

                        aspect.movementInformation.ValueRW.isMovementRequested = false;
                        aspect.movementInformation.ValueRW.isJumpAnimating = false;
                    }
                }

            }
        }
    }
}
