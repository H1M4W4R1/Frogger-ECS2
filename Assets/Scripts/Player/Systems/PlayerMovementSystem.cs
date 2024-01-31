using Audio.Authorings.SFX;
using Audio.Components;
using Levels.Components;
using LowLevel;
using Player.Aspects;
using Player.Components;
using Player.Systems.Enums;
using Player.Systems.Jobs;
using Unity.Burst;
using Unity.Entities;
using Unity.Jobs;
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
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Grab level width
            var onlySingleLevelFound = SystemAPI.TryGetSingleton(out LevelData levelData);
            var sfxData = SystemAPI.GetSingletonBuffer<SFXPlayerData>();

            // Process movement
            foreach((PlayerAspect aspect, Entity e) in SystemAPI.Query<PlayerAspect>().WithEntityAccess())
            {
                // Decode aspect information
                var movement = aspect.movement;
                var movementRO = aspect.movement.ValueRO;
                var movementInfo = aspect.movementInformation.ValueRO;

                // Automatically update level information
                if (onlySingleLevelFound)
                    movement.ValueRW.maxTilesToSide = levelData.levelHalfPlayableWidth;

                // Update component information
                SystemAPI.SetComponentEnabled<IsMoving>(e, movementInfo.isMoving);
                SystemAPI.SetComponentEnabled<IsMovementComputing>(e, movementInfo.isComputing);
                
                if (movementInfo.isComputing) return;

                if (!movementInfo.isAnimating)
                {
                    if(!movementInfo.isMoving)
                        new CheckForJumpAttemptJob().Schedule();
                    else
                    {
                        _jumpTimer = movementRO.jumpDistance / movementRO.jumpSpeed;
                        var sfxTracks = SystemAPI.GetBuffer<SFXTrack>(e);

                        sfxData.Add(new SFXPlayerData() 
                        {
                            sfxId = sfxTracks[PlayerSFX.JUMP].sfxId
                        });
                        
                        aspect.movementInformation.ValueRW.isAnimating = true;
                    }

                    // Handle player rotation during jump
                    new RotateFrogJob().Schedule();
                }
                else
                {
                    // Animate jump
                    var dt = SystemAPI.Time.DeltaTime;

                    // Jump animation
                    new AnimateFrogJumpJob(){jumpTimer = _jumpTimer}.Run();
                   
                    _jumpTimer -= dt;

                    if (_jumpTimer <= 0f)
                    {
                        // BUGFIX: when timer lags under 0f position will be terribly calculated, fix that using precalculated target position
                        aspect.localTransform.ValueRW.Position = movementInfo.startingPosition + movementInfo.movementVectorNonNormalized;

                        aspect.movementInformation.ValueRW.isMoving = false;
                        aspect.movementInformation.ValueRW.isAnimating = false;
                    }
                }

            }      
        }


    }
}
