using Microsoft.Extensions.Logging;
using TagLib;

namespace Meio.app.Services;

public class MetadataInfo
{
    public string? Title { get; set; }

    public string[]? Artists { get; set; }

    public string? Album { get; set; }

    public uint Year { get; set; }

    public string[]? Genres { get; set; }

    public byte[]? AlbumArt { get; set; }
}

public static class AudioMetadataService
{
    /// <summary>
    ///     Loads the metadata info of a given audio file.
    /// </summary>
    /// <param name="filePath">Path of the audio file to get the metadata from.</param>
    /// <returns>The metadata info of the audio file.</returns>
    public static MetadataInfo? LoadMetadata(string filePath)
    {
        try
        {
            var file = File.Create(filePath);
            var tag = file.Tag;

            App.Logger?.LogDebug("Asked for the metadata of {filePath}.", filePath);
            App.Logger?.LogTrace("Title: {tagTitle}", tag.Title);
            App.Logger?.LogTrace("Artists: {tagArtists}", string.Join(", ", tag.AlbumArtists));
            App.Logger?.LogTrace("Album: {tagAlbum}", tag.Album);
            App.Logger?.LogTrace("Year: {tagYear}", tag.Year);
            App.Logger?.LogTrace("Genre: {tagGenres}", string.Join(", ", tag.Genres));
            App.Logger?.LogTrace(tag.Pictures.Length > 0 ? "Got an album art." : "No album art was found.");

            return new MetadataInfo
            {
                Title = tag.Title,
                Artists = tag.AlbumArtists,
                Album = tag.Album,
                Genres = tag.Genres,
                Year = tag.Year,
                AlbumArt = tag.Pictures.Length > 0 ? tag.Pictures[0].Data.Data : null
            };
        }
        catch (CorruptFileException corruptFileException)
        {
            App.Logger?.LogError(corruptFileException, "Failed to load metadata. File is corrupted.");
            return null;
        }
        catch (UnsupportedFormatException unsupportedFormatException)
        {
            App.Logger?.LogError(unsupportedFormatException, "Failed to load metadata. File format is unsupported.");
            return null;
        }
    }
}