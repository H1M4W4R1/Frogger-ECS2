using System;
using DG.Tweening;
using UnityEngine;

namespace Audio.LowLevel
{
    public class Jukebox : MonoBehaviour
    {
        private static Jukebox _instance;
        
        public float trackChangeTime = 2.5f;
        
        // Audio management
        private AudioSource _playerA;
        private AudioSource _playerB;
        private AudioSource _currentPlayer;

        private UniqueAudioClip _currentTrack;

        private void Awake()
        {
            _instance = this;
            
            var sources = GetComponentsInChildren<AudioSource>();
            if (sources.Length < 2)
                throw new Exception("Jukebox requires two audio sources on its object");
            
            _playerA = sources[0];
            _playerB = sources[1];
        }

        public void CrossFade(AudioSource toMute, AudioSource toPlay, AudioClip clip)
        {
            toPlay.clip = clip;
            toPlay.Play();
                
            toMute.DOFade(0, trackChangeTime).OnComplete(toMute.Stop);
            toPlay.DOFade(1, trackChangeTime);

            _currentPlayer = toPlay;
        }
        
        public void UpdateTrack(UniqueAudioClip track)
        {
            if (_currentTrack != track)
            {
                // Play other track (Cross-fade tracks)
                if (_currentPlayer == _playerA)
                    CrossFade(_playerA, _playerB, AudioClipLibrary.GetClip(track));
                else
                    CrossFade(_playerB, _playerA, AudioClipLibrary.GetClip(track));
                
                _currentTrack = track;
            }
        }

        public static void PlayTrack(UniqueAudioClip track)
        {
            if (track < 0) return;
            if (!_instance) return;

            _instance.UpdateTrack(track);
        }

        public static UniqueAudioClip GetCurrentTrack()
        {
            if (!_instance) return UniqueAudioClip.None;
            return _instance._currentTrack;
        }
    }
}