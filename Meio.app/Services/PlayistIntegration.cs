using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
#if ANDROID || IOS
using Microsoft.Maui.Storage; // Needed for FileSystem.AppDataDirectory
#endif

namespace Meio.app.Services
{
    /// <summary>
    /// Represents a single audio track inside a playlist.
    /// Contains the file path and its extracted metadata.
    /// </summary>
    public class Track
    {
        public string FilePath { get; set; }      // Absolute path to the file
        public MetadataInfo Metadata { get; set; } // Metadata loaded via TagLib
    }

    /// <summary>
    /// A playlist is just a collection of tracks with a name.
    /// </summary>
    public class Playlist
    {
        public string Name { get; set; }             // Playlist name
        public List<Track> Tracks { get; set; } = new();
    }

    /// <summary>
    /// Service responsible for managing playlists:
    /// creation, adding tracks, saving and loading.
    /// </summary>
    public class PlaylistManager
    {
        private readonly List<Playlist> _playlists = new(); // Internal collection of playlists

        /// <summary>
        /// Returns all currently loaded playlists (read-only).
        /// </summary>
        public IReadOnlyList<Playlist> Playlists => _playlists;

        /// <summary>
        /// Creates a new playlist with a given name.
        /// </summary>
        public Playlist CreatePlaylist(string name)
        {
            var playlist = new Playlist { Name = name };
            _playlists.Add(playlist);
            return playlist;
        }

        /// <summary>
        /// Adds a new audio file to an existing playlist.
        /// Automatically extracts metadata using AudioMetadataService.
        /// </summary>
        public void AddTrackToPlaylist(Playlist playlist, string filePath)
        {
            if (!File.Exists(filePath)) return;

            var metadata = AudioMetadataService.LoadMetadata(filePath);
            if (metadata != null)
            {
                playlist.Tracks.Add(new Track
                {
                    FilePath = filePath,
                    Metadata = metadata
                });
            }
        }

        /// <summary>
        /// Builds a cross-platform path for saving playlists.
        /// - On Windows/Linux/macOS: uses the user's Music folder
        /// - On Android/iOS: uses FileSystem.AppDataDirectory
        /// </summary>
        private string GetPlaylistStoragePath(string playlistName)
        {
#if ANDROID || IOS
            return Path.Combine(FileSystem.AppDataDirectory, $"{playlistName}.json");
#else
            string musicFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            return Path.Combine(musicFolder, $"{playlistName}.json");
#endif
        }

        /// <summary>
        /// Saves a playlist as a JSON file in a cross-platform safe location.
        /// </summary>
        public void SavePlaylist(Playlist playlist)
        {
            string filePath = GetPlaylistStoragePath(playlist.Name);

            var json = JsonSerializer.Serialize(
                playlist,
                new JsonSerializerOptions { WriteIndented = true } // Human-readable
            );

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads a playlist from storage.
        /// Works with both desktop (Music folder) and mobile (AppDataDirectory).
        /// </summary>
        public Playlist LoadPlaylist(string playlistName)
        {
            string filePath = GetPlaylistStoragePath(playlistName);

            if (!File.Exists(filePath))
                throw new FileNotFoundException($"Playlist {playlistName} not found.", filePath);

            var json = File.ReadAllText(filePath);
            var playlist = JsonSerializer.Deserialize<Playlist>(json);

            if (playlist != null)
                _playlists.Add(playlist);

            return playlist!;
        }
    }
}
