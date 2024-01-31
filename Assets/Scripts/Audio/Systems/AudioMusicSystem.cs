using System;
using Audio.Components;
using Levels.Components;
using Unity.Burst;
using Unity.Entities;

namespace Audio.Systems
{
    public partial struct AudioMusicSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<MusicData>();
            state.RequireForUpdate<LevelAudioTrack>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var levelFound = SystemAPI.TryGetSingleton(out LevelAudioTrack levelData);
            if (levelFound)
            {
                // Update tracks for music data
                foreach (RefRW<MusicData> musicData in SystemAPI.Query<RefRW<MusicData>>())
                {
                    musicData.ValueRW.currentBackgroundTrack = levelData.track;
                }
            }
        }
    }
}