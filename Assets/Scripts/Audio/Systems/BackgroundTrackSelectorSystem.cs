using Audio.Components;
using Unity.Burst;
using Unity.Entities;

namespace Audio.Systems
{
    public partial struct BackgroundTrackSelectorSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BackgroundMusicInfo>();
            state.RequireForUpdate<LocalMusicTheme>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            if (SystemAPI.TryGetSingletonRW(out RefRW<BackgroundMusicInfo> bgMusic))
            {
                // Update audio tracks
                foreach (RefRO<LocalMusicTheme> theme in SystemAPI.Query<RefRO<LocalMusicTheme>>())
                {
                    var audioId = theme.ValueRO.audioId;
                    bgMusic.ValueRW.backgroundMusicIdentifier = audioId;
                }
            }
        }
    }
}