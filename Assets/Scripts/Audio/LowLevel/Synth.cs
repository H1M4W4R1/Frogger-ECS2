using System.Collections.Generic;
using System.Linq;
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

        private void Awake()
        {
            _instance = this;
        }
        
        public static AudioSource AcquireAudioSource()
        {
            if (!_instance) return null;
            var source = _instance._sources.FirstOrDefault(q => !q.isPlaying);

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
        
        public static void PlaySFX(UniqueAudioClip track)
        {
            if (track < 0) return;
            if (!_instance) return;

            var src = AcquireAudioSource();
            src.clip = AudioClipLibrary.GetClip(track);
            src.Play();
        }
    }
}