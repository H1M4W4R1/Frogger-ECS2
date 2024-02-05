using System;
using System.Collections.Generic;
using Management.Components;
using UnityEngine;
using Unity.Entities;
using Player.Components;
using Threats.Authorings.Data;
using Threats.Components;
using Unity.Collections;
using Unity.Mathematics;

namespace Threats.Authorings
{
    public class PlatformAuthoring : MonoBehaviour
    {
        public float speed;
        public float3 direction = new float3(-1, 0, 0);

        public List<PlatformSlot> offsets = new List<PlatformSlot>();

        private class Baker : Baker<PlatformAuthoring>
        {
            public override void Bake(PlatformAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent<IsPlatform>(e);
                AddComponent<IsPlayerOnPlatform>(e);
                AddComponent<IsPlatformUnderwater>(e);
                
                AddComponent(e, new MovingThreat()
                {
                    speed = authoring.speed,
                    direction = authoring.direction,
                    isNotNull = true,
                });

                // Register platform offsets
                var dBuffer = AddBuffer<PlatformOffsetsStore>(e);
                foreach (var offset in authoring.offsets)
                    dBuffer.Add(new PlatformOffsetsStore()
                    {
                        value = offset.position,
                        isDeathStore = offset.killsPlayer
                    });

                SetComponentEnabled<IsPlayerOnPlatform>(e, false);
                SetComponentEnabled<IsPlatformUnderwater>(e, false);
            }
        } 

        private void OnDrawGizmosSelected()
        {
            var tPos = transform.position;
            
            foreach (var oPos in offsets)
            {
                Gizmos.color = oPos.killsPlayer ? Color.red : Color.green;    
                Gizmos.DrawSphere(tPos + (Vector3) oPos.position, 0.1f);
            }
        }
    }
}
