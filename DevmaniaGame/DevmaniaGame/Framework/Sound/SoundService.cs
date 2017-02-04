using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Media;

namespace DevmaniaGame.Framework.Sound
{
    static class SoundService
    {
        private static string _currentSong;
        private static readonly Dictionary<string, Song> RegisteredSongs = new Dictionary<string, Song>(); 

        public static void RegisterSong(string songName, Song song)
        {
            if (RegisteredSongs.ContainsKey(songName))
                throw new InvalidOperationException(string.Format("Song with name '{0}' is already registered.", songName));

            RegisteredSongs.Add(songName, song);
        }

        public static void PlaySong(string songName, bool loop = true)
        {
            Song song;
            if (!RegisteredSongs.TryGetValue(songName, out song))
                throw new InvalidOperationException(string.Format("Song with name {0} does not exist.", songName));

            MediaPlayer.IsRepeating = loop;
            MediaPlayer.Play(song);
            MediaPlayer.Volume = 0.3f;
            _currentSong = songName;
        }

        public static bool IsPlaying(string songName)
        {
            return MediaPlayer.State == MediaState.Playing && _currentSong == songName;
        }

        public static void StopCurrentSong()
        {
            MediaPlayer.Stop();
        }
    }
}
