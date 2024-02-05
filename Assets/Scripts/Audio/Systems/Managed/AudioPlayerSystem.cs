using Audio.Components;
using Audio.LowLevel;
using Unity.Collections;
using Unity.Entities;

namespace Audio.Systems.Managed
{
    public partial class AudioPlayerSystem : SystemBase
    {
        private NativeList<int> _nList = new NativeList<int>(Allocator.Domain);

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

                    // If clip is not yet played
                    if (!alreadyPlayed)
                    {
                        // Play clip and register it
                        Synth.PlayVolumetricSFX(clip.sfxClip);
                        playedVolumetricSFXClips.Add(new PlayedVolumetricSFXInfo()
                        {
                            sfxClip = clip.sfxClip
                        });
                    }
                }
                
                // Clear non-played clips
                var index = 0;
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

                    // Stop playing the clip
                    if (!stillPlaying)
                    {
                        Synth.StopVolumetricSFX(clip.sfxClip);
                        _nList.Add(index);
                    }

                    index++;
                }
                
                // Clear old clips from stash
                foreach(var nIndex in _nList)
                    playedVolumetricSFXClips.RemoveAt(nIndex);

                _nList.Clear();
            }
        }
    }
}