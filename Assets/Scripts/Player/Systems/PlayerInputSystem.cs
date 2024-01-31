using Player.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Player.Systems
{
    public partial struct PlayerInputSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PlayerInput>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state) 
        {
            foreach (RefRW<PlayerInput> inputSystem in SystemAPI.Query<RefRW<PlayerInput>>())
            {
                var xAxis = Input.GetAxis("Horizontal");
                var yAxis = Input.GetAxis("Vertical");

                inputSystem.ValueRW.axisInput = new float2(xAxis, yAxis);
            }
            
        }
    }
}
