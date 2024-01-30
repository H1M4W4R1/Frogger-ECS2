using Assets.Scripts.Player.Components;
using Unity.Entities;
using UnityEngine;

namespace Assets.Scripts.Player.Authorings
{
    public class PlayerInputAuthoring : MonoBehaviour
    {

        private class Baker : Baker<PlayerInputAuthoring>
        {
            public override void Bake(PlayerInputAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new PlayerInput());
            }
        }
    }
}
