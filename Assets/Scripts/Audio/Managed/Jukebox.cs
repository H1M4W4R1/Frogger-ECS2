using System;
using System.Collections.Generic;
using Audio.Components;
using DG.Tweening;
using Unity.Burst;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Audio.Managed
{
    [BurstCompile]
    public class Jukebox : MonoBehaviour
    {
        #region SINGLETON
        private static Jukebox _instance;

        private static Jukebox Instance
        {
            get
            {
                if (!_instance)
                    _instance = FindFirstObjectByType<Jukebox>();

                return _instance;
            }
        }
        #endregion

        public float trackChangeTime = 10f;
        
        public readonly List<AudioClip> _tracks = new List<AudioClip>();
        
        // Audio management
        private AudioSource _playerA;
        private AudioSource _playerB;
        private AudioSource _currentPlayer; 

        private EntityManager _entityManager;
        private EntityQuery _entityQuery;
        
        private int _currentTrack = -100;
        
        private void Awake()
        {
            _instance = this;
            
            var sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
                throw new Exception("Jukebox requires two audio sources on its object");
            
            _playerA = sources[0];
            _playerB = sources[1];
            
            _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            _entityQuery = _entityManager.CreateEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    typeof(MusicData)
                }
            });
        }

        public static int RegisterClip(AudioClip clip)
        {
            Instance._tracks.RemoveAll(q => !q);
            if (Instance._tracks.Contains(clip)) return Instance._tracks.IndexOf(clip);           
           
            Instance._tracks.Add(clip);
            return Instance._tracks.Count - 1;
        }
        
        private void _Play(int track)
        {
            if (track == -1) return;
            
            // Play other track (Cross-fade tracks)
            if (_currentPlayer == _playerA)
            {
                _playerB.clip = _tracks[track];
                _playerB.Play();
                
                _playerA.DOFade(0, trackChangeTime).OnComplete(() =>
                {
                    _playerA.Stop();
                });
                _playerB.DOFade(1, trackChangeTime);

                _currentPlayer = _playerB;
            }
            else
            {
                _playerA.clip = _tracks[track];
                _playerA.Play();
                
                _playerA.DOFade(1, trackChangeTime);
                _playerB.DOFade(0, trackChangeTime).OnComplete(() =>
                {
                    _playerB.Stop();
                });
                
                _currentPlayer = _playerA;
            }

            _currentTrack = track;
        }

        private void LateUpdate()
        {
            // Get entity
            var targetEntity = _entityQuery.GetSingletonEntity();

            // Play music track ;)
            if (_entityManager.HasComponent<MusicData>(targetEntity))
            {
                var data = _entityManager.GetComponentData<MusicData>(targetEntity);
                
                if(_currentTrack != data.currentBackgroundTrack)
                    _Play(data.currentBackgroundTrack);
            }
        }
    }
}