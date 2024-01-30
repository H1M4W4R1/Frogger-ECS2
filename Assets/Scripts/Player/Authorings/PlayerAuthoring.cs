using Unity.Entities;
using UnityEngine;

namespace Player.Authorings
{
    public class PlayerAuthoring : MonoBehaviour
    {

        private class Baker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new Components.Player());
            }
        }

    }
}
