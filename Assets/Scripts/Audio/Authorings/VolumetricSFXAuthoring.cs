using System;
using Audio.Components;
using Audio.Components.Managed;
using Audio.LowLevel;
using UnityEngine;
using Unity.Entities;
using Player.Components;
using Unity.Mathematics;

namespace Audio.Authorings
{
    public class VolumetricSFXAuthoring : MonoBehaviour
    {
        public ManagedAudioClip clip;
        public float3 scale;
        
        private class Baker : Baker<VolumetricSFXAuthoring>
        {
            public override void Bake(VolumetricSFXAuthoring authoring)
            {
                var e = GetEntity(TransformUsageFlags.None);
                AddComponent(e, new VolumetricSFXVolume()
                {
                    clip = authoring.clip.uniqueId,
                    center = authoring.transform.position,
                    scale = authoring.scale
                });
                AddComponent<IsPlayerInsideVolume>(e);
                SetComponentEnabled<IsPlayerInsideVolume>(e, false);
                
                // Register managed clips (it will be processed by system)
                var clipsComponent = new AudioClips();
                clipsComponent.clips.Add(authoring.clip);
                AddComponentObject(e, clipsComponent);
                
                AddComponent<ClipsRegistered>(e);
                SetComponentEnabled<ClipsRegistered>(e, false);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawCube(transform.position, scale);
        }
    }
}
