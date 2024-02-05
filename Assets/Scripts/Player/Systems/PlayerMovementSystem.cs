using System;
using Audio.Components;
using Audio.LowLevel;
using Helpers;
using Levels.Components;
using LowLevel;
using Player.Aspects;
using Player.Components;
using Player.Systems.Jobs;
using Threats.Components;
using Threats.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
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
                        
                        // Compute and indicate target position
                        var lPos = movementInfo.startingPosition + movementInfo.movementVectorNonNormalized;
                        if (movementInfo.movementVectorNonNormalized is {x: 0, z: 0}) return;
                        aspect.movementInformation.ValueRW.lastJumpTarget = lPos;

                        // Check tiles
                        AcquireTileAtPositionJob.Prepare(out var tileAccessJob, lPos.x, lPos.z);
                        tileAccessJob.Schedule(state.Dependency).Complete();
                        
                        // Update tile data
                        var tileType = tileAccessJob.GetFoundTileType();
                        SystemAPI.SetComponentEnabled<IsTargetTileNull>(e, tileType == (byte) LevelTiles.None);
                        
                        // Player has jumped - remove platform assignments
                        foreach (var (platform, e1) in SystemAPI.Query<MovingThreat>()
                                     .WithAll<IsPlayerOnPlatform, IsPlatform>().WithEntityAccess())
                        {
                            SystemAPI.SetComponentEnabled<IsPlayerOnPlatform>(e1, false);
                        }
                        
                        aspect.movementInformation.ValueRW.willBeDead = false;
                        
                        // Check if isKillTile
                        if (tileAccessJob.IsKillTile())
                        {
                            // Get nearby platform
                            AcquireNearestPlatformJob.Prepare(out var nearestPlatformJob, lPos);
                            nearestPlatformJob.Schedule(state.Dependency).Complete();

                            // Jump to platform
                            if (!nearestPlatformJob.IsPlatformNull())
                            {
                                // Get platform position
                                var platformPosition = nearestPlatformJob.GetPosition();

                                // Get offsets at current platform
                                var offsets =
                                    SystemAPI.GetBuffer<PlatformOffsetsStore>(nearestPlatformJob.platformEntity[0]);
                             
                                // Get nearest offset
                                var nOffsetPos = float3.zero;
                                var nOffsetDistanceSquare = 10e3f;
                                var offsetDataRO = default(PlatformOffsetsStore);

                                foreach (var offset in offsets)
                                {
                                    // Compute difference between platform offset and local position of player
                                    var diff = platformPosition + offset.value - lPos;
                                    var dist = diff.x * diff.x + diff.z * diff.z;

                                    if (dist < nOffsetDistanceSquare)
                                    {
                                        nOffsetDistanceSquare = dist;
                                        nOffsetPos = offset.value;
                                        offsetDataRO = offset;
                                    }
                                }

                                // Check if current offset is near frog jump, value equal to tile size is okay for nice UX
                                if (nOffsetDistanceSquare < ConstConfig.TILE_SIZE)
                                {
                                    // Fix jump vector against platform position
                                    aspect.movementInformation.ValueRW.lastJumpTarget =
                                        platformPosition + nOffsetPos;

                                    // Mark as player is on platform
                                    SystemAPI.SetComponentEnabled<IsPlayerOnPlatform>(
                                        nearestPlatformJob.platformEntity[0], true);

                                    // Attempt jumping onto platform
                                    AttemptJumpJob.Prepare(out var attemptJump, tileType, 1);
                                    attemptJump.Schedule(state.Dependency).Complete();
                                    
                                    // Check for death platforms (like gator jaws)
                                    if (offsetDataRO.isDeathStore)
                                        aspect.movementInformation.ValueRW.willBeDead = true;

                                    SystemAPI.SetComponentEnabled<IsOnPlatform>(e, true);
                                }
                                else // Death jump
                                {
                                    // Regular death jump
                                    AttemptJumpJob.Prepare(out var attemptJump, tileType, 0);
                                    attemptJump.Schedule(state.Dependency).Complete();
                                    SystemAPI.SetComponentEnabled<IsOnPlatform>(e, false);
                                    
                                    aspect.movementInformation.ValueRW.willBeDead = true;
                                }
                                
                                // Clear job memory
                                state.Dependency = nearestPlatformJob.Dispose(state.Dependency);
                            }
                            else
                            { 
                                // Regular death jump
                                AttemptJumpJob.Prepare(out var attemptJump, tileType, 0);
                                attemptJump.Schedule(state.Dependency).Complete();
                                SystemAPI.SetComponentEnabled<IsOnPlatform>(e, false);

                                aspect.movementInformation.ValueRW.willBeDead = true;
                            }
                        }
                        else
                        {
                            // Attempt jumping onto tile
                            AttemptJumpJob.Prepare(out var attemptJump, tileType, 0);
                            attemptJump.Schedule(state.Dependency).Complete();
                            SystemAPI.SetComponentEnabled<IsOnPlatform>(e, false);
                        }

                        // Free memory
                        state.Dependency = tileAccessJob.Dispose(state.Dependency);
                    }
                    else
                    {
                        // Recompute new jump vector to prevent issues
                        aspect.movementInformation.ValueRW.movementVectorNonNormalized =
                            movementInfo.lastJumpTarget - movementInfo.startingPosition;
                        
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
                    job.Schedule(state.Dependency).Complete();

                    _jumpTimer -= dt;

                    if (SystemAPI.IsComponentEnabled<IsDead>(e))
                    {
                        // Reset movement params
                        aspect.movementInformation.ValueRW.isMovementRequested = false;
                        aspect.movementInformation.ValueRW.isJumpAnimating = false;
                        aspect.movementInformation.ValueRW.willBeDead = false;
                        aspect.movementInformation.ValueRW.movementVectorNonNormalized = float3.zero;

                        _jumpTimer = 0f;
                    }
                    else // If not dead then process
                    {
                        if (_jumpTimer <= 0f)
                        {
                            // BUGFIX: when timer lags under 0f position will be terribly calculated, fix that using precalculated target position
                            aspect.localTransform.ValueRW.Position =
                                movementInfo.startingPosition + movementInfo.movementVectorNonNormalized;

                            aspect.movementInformation.ValueRW.isMovementRequested = false;
                            aspect.movementInformation.ValueRW.isJumpAnimating = false;
 
                            // Kill player if will be dead after move
                            if (aspect.movementInformation.ValueRO.willBeDead)
                            {
                                SystemAPI.SetComponentEnabled<IsDead>(e, true);
                                aspect.movementInformation.ValueRW.willBeDead = false;
                            
                                // Bugfix for quick direction change while frog is still performing death jump, do not touch.
                                aspect.movementInformation.ValueRW.movementVectorNonNormalized = float3.zero;
                            }
                        }
                    }
                }

            }
        }
    }
}
