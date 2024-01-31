using System;
using System.Collections.Generic;
using System.Linq;
using Audio.Components;
using NUnit.Framework;
using Unity.Collections;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

namespace Audio.Managed
{
    public class Synth : MonoBehaviour
    {
        private const int N_CHANNELS = 16;
        
        #region SINGLETON
        private static Synth _instance;

        private static Synth Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindFirstObjectByType<Synth>();

                return _instance;
            }
        }
        #endregion 

        [SerializeField] [ReadOnly] private readonly List<AudioClip> _sfxClips = new List<AudioClip>();
        private readonly List<AudioSource> _sources = new List<AudioSource>();
        private Queue<int> _audioSFXToPlay = new Queue<int>();

        private bool _initialized;
        
        public AudioMixer mixer;
        public string mixerGroup = "SFX";
       
        private EntityManager _entityManager;
        private EntityQuery _entityQuery;

        private void Awake()
        {
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entityQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(SFXPlayerData)
                }
            });
        }

        public static int AddSFX(AudioClip clip)
        {
            Instance._sfxClips.RemoveAll(q => !q);
            if (Instance._sfxClips.Contains(clip)) return Instance._sfxClips.IndexOf(clip);            
            
            Instance._sfxClips.Add(clip);
            return Instance._sfxClips.Count - 1;
        }

        public static void Play(int sfx) =>
            Instance._audioSFXToPlay.Enqueue(sfx);

        public static AudioSource AcquireAudioSource()
        {
            var source = Instance._sources.FirstOrDefault(q => !q.isPlaying);

            if (!source)
            {
                source = Instance.AddComponent<AudioSource>();
                source.outputAudioMixerGroup = Instance.mixer
                    .FindMatchingGroups(Instance.mixerGroup)
                    .FirstOrDefault();
                Instance._sources.Add(source);
            }

            return source;
        }

        public void Update()
        {
            // Get entity
            var targetEntity = _entityQuery.GetSingletonEntity();

            // Play all SFX ;)
            if (_entityManager.HasBuffer<SFXPlayerData>(targetEntity))
            {
                var data = _entityManager.GetBuffer<SFXPlayerData>(targetEntity);
                
                foreach(var d in data)
                    Play(d.sfxId);
                
                // Might lose some plays, but don't care
                data.Clear();
            }
            
            // Play audio SFX
            while (_audioSFXToPlay.Count > 0)
            {
                var track = _audioSFXToPlay.Dequeue();
                var source = AcquireAudioSource();
                source.clip = _sfxClips[track];
                source.Play();
            }
        }
    }
}