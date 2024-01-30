using System;
using Levels.Components;
using UnityEngine;
using Unity.Entities;
using Player.Components;

namespace Levels.Authorings
{
    public class LevelAuthoring : MonoBehaviour
    {
        private class Baker : Baker<LevelAuthoring>
        {
            public override void Bake(LevelAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelTag());
            }
        }
    }
}
