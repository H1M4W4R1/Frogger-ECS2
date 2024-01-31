using System;
using System.Collections.Generic;
using Audio.Components;
using Audio.Managed;
using UnityEngine;
using Unity.Entities;
using Player.Components;

namespace Audio.Authorings
{

    public abstract class SFXTrackAuthoringBase : MonoBehaviour
    {
        private List<AudioClip> _clips = new List<AudioClip>();
        private int _nClip;

        protected void AddClip(AudioClip clip, int index)
        {
            if (index != _nClip)
                Debug.LogWarning($"{index} does not meet clip index {_nClip}. Are you doing something in wrong order?");
            else
            {
                _clips.Add(clip);
                _nClip++;
            }
        }

        protected abstract void _RegisterClips();

        protected void RegisterClips<T>(DynamicBuffer<T> buffer) where T : unmanaged
        {
            buffer.Clear();
            _RegisterClips();
        }
        
        public void Bake<T>(Baker<T> baker, T authoring) where T : SFXTrackAuthoringBase
        {
            var e = baker.GetEntity(TransformUsageFlags.None);
            var buffer = baker.AddBuffer<SFXTrack>(e);
            
            authoring.RegisterClips(buffer);

            // Register all clips in this object
            foreach (var clip in authoring._clips)
            {
                buffer.Add(new SFXTrack()
                {
                    sfxId = Synth.AddSFX(clip)
                });
            }
        }

        
    }
}
