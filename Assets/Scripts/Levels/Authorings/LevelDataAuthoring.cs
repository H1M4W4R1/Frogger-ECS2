using System;
using Levels.Components;
using UnityEngine;
using Unity.Entities;
using Player.Components;
using UnityEngine.Serialization;

namespace Levels.Authorings
{
    public class LevelDataAuthoring : MonoBehaviour
    {
        public float timeUntilFailed;
        public float threatSpeedMultiplier;
        
        private class Baker : Baker<LevelDataAuthoring>
        {
            public override void Bake(LevelDataAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new LevelData()
                {
                   time = authoring.timeUntilFailed,
                   threatSpeed = authoring.threatSpeedMultiplier
                });
            }
        }

        
    }
}
