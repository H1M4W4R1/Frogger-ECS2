using Player.Components;
using Threats.Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace Threats.Systems
{
    public partial struct PlayerPlatformStickSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            foreach (var lThreat in SystemAPI.Query<RefRO<MovingThreat>>().WithAll<IsPlatform, IsPlayerOnPlatform>())
            {
                foreach (var lTransform in SystemAPI.Query<RefRW<LocalTransform>>().WithAll<PlayerTag>())
                {
                    lTransform.ValueRW.Position += SystemAPI.Time.DeltaTime * lThreat.ValueRO.speed * lThreat.ValueRO.direction;
                }
         
            }
        }

    }
}