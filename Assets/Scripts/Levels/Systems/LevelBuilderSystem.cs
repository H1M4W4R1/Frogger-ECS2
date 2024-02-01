using Levels.Aspects;
using Levels.Components;
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
            foreach (var tile in SystemAPI.Query<TileAspect>())
            {
                var pos = tile.localTransform.ValueRO.Position;
                tile.render.ValueRW.xPosition = (int) pos.x;
                tile.render.ValueRW.zPosition = (int) pos.z;

                tile.render.ValueRW.tileId = tile.tile.ValueRO.tileType;
            }
        }
    }
}