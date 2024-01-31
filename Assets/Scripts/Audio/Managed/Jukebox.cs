using System;
using System.Collections.Generic;
using Audio.Managed.Components;
using DG.Tweening;
using Unity.Burst;
using UnityEngine;

namespace Audio.Managed
{
    [BurstCompile]
    public class Jukebox : MonoBehaviour
    {
        public float trackChangeTime = 10f;
        
        public List<MusicTrack> tracks = new List<MusicTrack>();
        
        // Audio management
        private AudioSource _playerA;
        private AudioSource _playerB;
        private AudioSource _currentPlayer;

        private MusicTrack _currentTrack;
        private static MusicTrack _trackToPlay;
        
        private void Awake()
        {
            var sources = GetComponents<AudioSource>();
            if (sources.Length < 2)
                throw new Exception("Jukebox requires two audio sources on its object");
            
            _playerA = sources[0];
            _playerB = sources[1];
        }

        private void _Play(MusicTrack track)
        {
            // Play other track (Cross-fade tracks)
            if (_currentPlayer == _playerA)
            {
                _playerB.clip = track.clip;
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
                _playerA.clip = track.clip;
                _playerA.Play();
                
                _playerA.DOFade(1, trackChangeTime);
                _playerB.DOFade(0, trackChangeTime).OnComplete(() =>
                {
                    _playerB.Stop();
                });
                
                _currentPlayer = _playerA;
            }
            

            _currentTrack = _trackToPlay;
        }
        
        public static void Play(MusicTrack track)
        {
            _trackToPlay = track;
        }

        private void Update()
        {
            if (_currentTrack != _trackToPlay)
                _Play(_trackToPlay);
        }
    }
}