using System.Collections.Generic;
using UnityEngine;

namespace Audio.LowLevel
{
    public class AudioClipLibrary : MonoBehaviour
    {
        private static AudioClipLibrary _instance;
        
        private readonly List<RegisteredAudioClip> _clips = new List<RegisteredAudioClip>();

        private void Awake()
        {
            _instance = this;
        }

        public static int AddClip(AudioClip clip, UniqueAudioClip clipEventIdentifier)
        {
            if (!_instance) return -1;

            var clips = _instance._clips;
            
            var index = clips.FindIndex(c => c.clipEventIdentifier == clipEventIdentifier);
            if (index >= 0) return index;
            
            var clipObj = new RegisteredAudioClip
            {
                clipEventIdentifier = clipEventIdentifier,
                clip = clip
            };

            clips.Add(clipObj);
            return clips.Count - 1;
        }

        public static AudioClip GetClip(UniqueAudioClip uniqueIndex)
        {
            if (!_instance) return null;
            var clips = _instance._clips;

            var index = clips.FindIndex(c => c.clipEventIdentifier == uniqueIndex);
            if(index < 0)
                Debug.LogError($"Clip with unique index {uniqueIndex} was not found :(");

            return clips[index].clip;
        }
    }
}