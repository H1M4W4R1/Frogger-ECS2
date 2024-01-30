using Assets.Scripts.Player.Components;
using Unity.Entities;
using Unity.Transforms;

namespace Assets.Scripts.Player.Aspects
{
    public readonly partial struct PlayerAspect : IAspect
    {
        public readonly RefRO<Components.Player> playerTAG;
        
        public readonly RefRW<LocalTransform> localTransform;
        public readonly RefRO<PlayerMovement> movement;
        public readonly RefRO<PlayerInput> input;

        // ref: PlayerMovementSystem
    }
}
