using Player.Components;
using Unity.Entities;
using UnityEngine;

namespace Player.Authorings
{
    public class PlayerAuthoring : MonoBehaviour
    {
        // Jumping - player position change
        public float jumpDistance = 2;
        public float jumpHeight = 0.5f;
        public float jumpSpeed = 9;

        // If true then player character will rotate after jumping (if jumped left player will rotate left etc.)
        public bool rotatePlayerCharacter = true;

        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                
                // Tags
                AddComponent(e, new PlayerTag());
                AddComponent<IsTargetTileNull>(e);
                AddComponent<IsStandingStill>(e);
                
                AddComponent(e, new IsMovementComputing());
                AddComponent(e, new HasMovementRequest());
                
                // Movement System
                AddComponent(e, new PlayerInput());
                AddComponent(e, new PlayerMovementSettings
                {
                    jumpDistance = authoring.jumpDistance,
                    jumpHeight = authoring.jumpHeight,
                    jumpSpeed = authoring.jumpSpeed,
                    rotatePlayerCharacter = authoring.rotatePlayerCharacter,
                });
           
                AddComponent(e, new CurrentMovementData());
         
                // Disable components
                SetComponentEnabled<IsTargetTileNull>(e, false);
                SetComponentEnabled<IsStandingStill>(e, false);
                SetComponentEnabled<IsMovementComputing>(e, false);
                SetComponentEnabled<HasMovementRequest>(e, false);
            }
        }

    }
}
