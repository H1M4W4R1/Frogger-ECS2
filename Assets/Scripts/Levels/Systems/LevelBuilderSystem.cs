﻿using Levels.Components;
using LowLevel;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using LevelAspect = Levels.Aspects.LevelAspect;

namespace Levels.Systems
{
    public partial struct LevelBuilderSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelTag>();
            state.RequireForUpdate<LevelBuiltTag>();
            state.RequireForUpdate<LevelData>();
            state.RequireForUpdate<TileLibrary>();
            state.RequireForUpdate<RenderedLevelTile>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var lib = SystemAPI.GetSingletonBuffer<TileLibrary>();
            var cmdBuffer = new EntityCommandBuffer(Allocator.Domain);
            
            foreach (var (loadedLevel, entity) in SystemAPI.Query<LevelAspect>()
                         .WithDisabled<LevelBuiltTag>()
                         .WithAll<BuildableLevelTag>()
                         .WithEntityAccess())
            {
                var levelData = loadedLevel.levelData.ValueRO;
                var grassTile = lib[0].tile;
                var renderedData = SystemAPI.GetBuffer<RenderedLevelTile>(entity);

                // TODO: Load JSON and job-build the level
                for (var x = -levelData.levelHalfRenderedWidth * ConstConfig.TILE_SIZE;
                     x <= levelData.levelHalfRenderedWidth * ConstConfig.TILE_SIZE; 
                     x += ConstConfig.TILE_SIZE)
                {
                    for(var z = 0; z <= 256; z += ConstConfig.TILE_SIZE)
                    {
                        var spawnedObject = cmdBuffer.Instantiate(grassTile);
                        cmdBuffer.SetComponent(spawnedObject, new LocalTransform()
                        {
                            Position = new float3(x, 0, z),
                            Rotation = quaternion.EulerXYZ(90 * Mathf.Deg2Rad, 0, 0),
                            Scale = 2f
                        });

                        renderedData.Add(new RenderedLevelTile()
                        {
                            tileId = 0,
                            xPosition = x,
                            zPosition = z
                        });
                    }
                }
                
                SystemAPI.SetComponentEnabled<LevelBuiltTag>(entity, true);
            }
            
            // Build level data
            cmdBuffer.Playback(state.EntityManager);
        }
    }
}