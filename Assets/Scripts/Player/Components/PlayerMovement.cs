
using Unity.Entities;

namespace Player.Components
{
    public struct PlayerMovement : IComponentData
    {
        // Jumping - player position change
        public float jumpDistance;
        public float jumpHeight;
        public float jumpSpeed;
        
        // Max tiles player can move to side
        public int maxTilesToSide;

        // If true then player character will rotate after jumping (if jumped left player will rotate left etc.)
        public bool rotatePlayerCharacter;   

    }
}
