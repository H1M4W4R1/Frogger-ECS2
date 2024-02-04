using Helpers;
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
    public partial struct LevelTileComputationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LevelTag>();
            state.RequireForUpdate<LevelData>();
            state.RequireForUpdate<Tile>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var tile in SystemAPI.Query<TileAspect>())
            {
                var pos = tile.localTransform.ValueRO.Position;
                tile.render.ValueRW.xPosition = MathHelper.TileInt(pos.x);
                tile.render.ValueRW.zPosition = MathHelper.TileInt(pos.z);
            }
        }
    }
}