using Audio.Components;
using Audio.LowLevel;
using Helpers;
using Levels.Components;
using LowLevel;
using Player.Aspects;
using Player.Components;
using Player.Systems.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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

                if (movementInfo.isMovementComputing) return;

                if (!movementInfo.isJumpAnimating)
                {
                    if (!movementInfo.isMovementRequested)
                    {
                        // Compute vector
                        new ComputeJumpVectorJob().Schedule(state.Dependency).Complete();
                        lPos += movementInfo.movementVectorNonNormalized;

                        // Skip empty vector
                        if (movementInfo.movementVectorNonNormalized is {x: 0, z: 0}) return;

                        // Check tiles
                        var tileAccessJob = new AcquireTileAtPositionJob()
                        {
                            x = MathHelper.TileInt(lPos.x),
                            z = MathHelper.TileInt(lPos.z),
                            foundTileType = new NativeArray<byte>(1, Allocator.Domain)
                        };
                        tileAccessJob.Schedule(state.Dependency).Complete();

                        var tileType = tileAccessJob.foundTileType[0];
                        SystemAPI.SetComponentEnabled<IsTargetTileNull>(e, tileType == (byte) LevelTiles.None);

                        // Attempt jumping onto tile
                        new AttemptJumpJob() {foundTileId = tileType}
                            .Schedule(state.Dependency)
                            .Complete();

                        // Free memory
                        state.Dependency = tileAccessJob.foundTileType.Dispose(state.Dependency);
                    }
                    else
                    {
                        _jumpTimer = movementRO.jumpDistance / movementRO.jumpSpeed;
                        aspect.movementInformation.ValueRW.isJumpAnimating = true;

                        // Handle player rotation during jump
                        state.Dependency = new RotateFrogJob().Schedule(state.Dependency);

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
                    new AnimateFrogJumpJob() {jumpTimer = _jumpTimer}.Run();

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
