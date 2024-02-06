using Player.Aspects;
using Player.Components;
using Threats.Components;
using Unity.Burst;
using Unity.Entities;

namespace Player
{
    public partial struct PlayerPlatformWaterDeathSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Get player
            if (SystemAPI.TryGetSingletonEntity<PlayerTag>(out var player))
            {
                // If any platform with player is underwater
                foreach (var platform in SystemAPI.Query<IsPlatform>()
                             .WithAll<IsPlatformUnderwater, IsPlayerOnPlatform>())
                {
                    // You are dead.
                    SystemAPI.SetComponentEnabled<IsDead>(player, true);
                    break;
                }
            }
        }
    }
}