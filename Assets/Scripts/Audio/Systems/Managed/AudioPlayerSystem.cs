using Audio.Components;
using Audio.LowLevel;
using Unity.Entities;

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

            if (SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<VolumetricSFXInfo> volumetricSFXClips) &&
                SystemAPI.TryGetSingletonBuffer(out DynamicBuffer<PlayedVolumetricSFXInfo> playedVolumetricSFXClips))
            {
                foreach (var clip in volumetricSFXClips)
                {
                    // Check if clip is already played
                    var alreadyPlayed = false;
                    foreach (var playedClip in playedVolumetricSFXClips)
                    {
                        if (playedClip.sfxClip == clip.sfxClip)
                        {
                            alreadyPlayed = true;
                            break;
                        }
                    }
                    
                    if(!alreadyPlayed)
                        Synth.PlayVolumetricSFX(clip.sfxClip);
                }
                
                // Clear non-played clips
                foreach (var clip in playedVolumetricSFXClips)
                {
                    // Check if clip is still playing
                    var stillPlaying = false;
                    foreach (var playedClip in volumetricSFXClips)
                    {
                        if (playedClip.sfxClip == clip.sfxClip)
                        {
                            stillPlaying = true;
                            break;
                        }
                    }
                    
                    if(!stillPlaying)
                        Synth.StopVolumetricSFX(clip.sfxClip);
                }
            }
        }
    }
}