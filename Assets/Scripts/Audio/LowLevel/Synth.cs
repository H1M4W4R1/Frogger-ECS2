using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio.LowLevel
{
    public class Synth : MonoBehaviour
    {
        private static Synth _instance;

        public AudioMixer mixer;
        public string group = "SFX";
        public float trackChangeTime = 2.5f;
        
        // Audio management
        private readonly List<AudioSource> _sources = new List<AudioSource>();

        private readonly List<AudioSource> _volumetricSFXSources = new List<AudioSource>();
        
        private void Awake()
        {
            _instance = this;
        }
        
        public static AudioSource AcquireAudioSource()
        {
            if (!_instance) return null;
            
            // Acquire non-playing non-volumetric (atm) source
            var source = _instance._sources.FirstOrDefault(q => !q.isPlaying &&
                                                                !_instance._volumetricSFXSources.Contains(q));

            // If source was not found create one
            if (!source)
            {
                source = _instance.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _instance.mixer
                    .FindMatchingGroups(_instance.group)
                    .FirstOrDefault();
                _instance._sources.Add(source);
            }

            return source;
        }

        public static void PlayVolumetricSFX(UniqueAudioClip track)
        {
            if (track < 0) return;
            if (!_instance) return;

            // Acquire audio source and setup data for VSFX
            var volumetricSFX = AcquireAudioSource();
            volumetricSFX.volume = 0f;
            volumetricSFX.clip = AudioClipLibrary.GetClip(track);
            volumetricSFX.loop = true;
            volumetricSFX.Play();
  
            volumetricSFX.DOFade(1f, _instance.trackChangeTime);
            
            // Register volumetric sfx source and track
            _instance._volumetricSFXSources.Add(volumetricSFX);
        }

        public static void StopVolumetricSFX(UniqueAudioClip track)
        {
            if (track < 0) return;
            if (!_instance) return;
            var clip = AudioClipLibrary.GetClip(track);
      
            // Disable volumetric sfx
            var volumetricSFX = _instance._volumetricSFXSources.FirstOrDefault(src => src.clip == clip);
            if (volumetricSFX)
            {
                // Fade to zero audio
                volumetricSFX.DOFade(0f, _instance.trackChangeTime)
                    .OnComplete(() =>
                    {
                        volumetricSFX.loop = false;
                        volumetricSFX.Stop();
                        
                        // Remove source instance
                        _instance._volumetricSFXSources.Remove(volumetricSFX);
                    });
            }
        }
        
        public static void PlaySFX(UniqueAudioClip track)
        {
            if (track < 0) return;
            if (!_instance) return;

            var src = AcquireAudioSource();
            src.clip = AudioClipLibrary.GetClip(track);
            src.volume = 1f; // Full-blown volume ;)
            src.Play();
        }
    }
}