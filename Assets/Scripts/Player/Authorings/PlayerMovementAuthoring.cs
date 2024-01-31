using Player.Components;
using Unity.Entities;
using UnityEngine;

namespace Player.Authorings
{
    public class PlayerMovementAuthoring : MonoBehaviour
    {
        // Jumping - player position change
        public float jumpDistance;
        public float jumpHeight;
        public float jumpSpeed;

        public int maxTilesToSize = 3;
        
        // If true then player character will rotate after jumping (if jumped left player will rotate left etc.)
        public bool rotatePlayerCharacter;

        private class Baker : Baker<PlayerMovementAuthoring>
        {
            public override void Bake(PlayerMovementAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(e, new PlayerMovement
                {
                    jumpDistance = authoring.jumpDistance,
                    jumpHeight = authoring.jumpHeight,
                    jumpSpeed = authoring.jumpSpeed,
                    rotatePlayerCharacter = authoring.rotatePlayerCharacter,
                    maxTilesToSide = authoring.maxTilesToSize
                });
            }
        }
    }
}
