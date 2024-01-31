using UnityEngine;

namespace Audio.Managed.Components
{
    [CreateAssetMenu(fileName = "Data/Audio/Track.asset", menuName = "Audio/Music Track", order = 1)]
    public class MusicTrack : ScriptableObject
    {
        public AudioClip clip;
    }
}