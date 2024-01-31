using Audio.Components;
using Audio.LowLevel;
using Unity.Entities;
using UnityEngine;

namespace Audio.Systems.Managed
{
    public partial class AudioPlayerSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<BackgroundMusicInfo>();
        }

        protected override void OnUpdate()
        {
            if (SystemAPI.TryGetSingleton(out BackgroundMusicInfo bmi))
            {
                var id = bmi.backgroundMusicIdentifier;
                if (Jukebox.GetCurrentTrack() != id)
                    Jukebox.PlayTrack(id);
            }

            if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<SFXInfo> sfxClips))
            {
                foreach (var clip in sfxClips)
                    Synth.PlaySFX(clip.sfxClip);
                
                sfxClips.Clear();
            }
        }
    }
}