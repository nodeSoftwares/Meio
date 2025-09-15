using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Meio.app.Services
{
    /// <summary>
    /// Represents a single audio track inside a playlist.
    /// It contains both the file path and the extracted metadata.
    /// </summary>
    public class Track
    {
        public string FilePath { get; set; }      // Absolute path of the audio file
        public MetadataInfo Metadata { get; set; } // Metadata loaded from the file (title, artist, etc.)
    }

    /// <summary>
    /// Represents a playlist, which is basically a collection of tracks with a name.
    /// </summary>
    public class Playlist
    {
        public string Name { get; set; }           // Playlist name (e.g. "My Favorites")
        public List<Track> Tracks { get; set; } = new(); // List of tracks in this playlist
    }

    /// <summary>
    /// Manages multiple playlists: creation, adding tracks, saving and loading.
    /// </summary>
    public class PlaylistManager
    {
        private readonly List<Playlist> _playlists = new(); // Internal storage of playlists

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
        /// Metadata is automatically extracted using AudioMetadataService.
        /// </summary>
        /// <param name="playlist">Target playlist</param>
        /// <param name="filePath">Absolute path of the audio file</param>
        public void AddTrackToPlaylist(Playlist playlist, string filePath)
        {
            if (!File.Exists(filePath)) return; // Ensure the file exists

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
        /// Saves a playlist as a JSON file.
        /// This makes it persistent and easy to reload later.
        /// </summary>
        public void SavePlaylist(Playlist playlist, string filePath)
        {
            var json = JsonSerializer.Serialize(
                playlist,
                new JsonSerializerOptions { WriteIndented = true } // Makes the JSON human-readable
            );

            File.WriteAllText(filePath, json);
        }

        /// <summary>
        /// Loads a playlist from a JSON file.
        /// The playlist is also added to the manager's internal collection.
        /// </summary>
        public Playlist LoadPlaylist(string filePath)
        {
            var json = File.ReadAllText(filePath);
            var playlist = JsonSerializer.Deserialize<Playlist>(json);

            if (playlist != null)
                _playlists.Add(playlist);

            return playlist!;
        }
    }
}

/*
Exemple of usage : 
var manager = new PlaylistManager();
   
   // Create a new playlist
   var myPlaylist = manager.CreatePlaylist("My Favorite Songs");
   
   // Add audio tracks
   manager.AddTrackToPlaylist(myPlaylist, @"C:\Music\song1.mp3");
   manager.AddTrackToPlaylist(myPlaylist, @"C:\Music\song2.flac");
   
   // Save the playlist to disk (as JSON)
   manager.SavePlaylist(myPlaylist, @"C:\Playlists\my_playlist.json");
   
   // Load the playlist back from disk
   var loaded = manager.LoadPlaylist(@"C:\Playlists\my_playlist.json");
   
   // Print all track titles and artists
   foreach (var track in loaded.Tracks)
   {
       Console.WriteLine($"{track.Metadata.Title} - {string.Join(", ", track.Metadata.Artists ?? new string[]{})}");
   } */