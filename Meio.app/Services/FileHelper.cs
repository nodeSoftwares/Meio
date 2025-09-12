using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Logging;

namespace Meio.app.Services;

public static class FileHelper
{
    /// <summary>
    ///     Opens a file box dialog, to get its path.
    /// </summary>
    /// <param name="topLevel">TopLevel of the given visual.</param>
    /// <returns></returns>
    public static async Task<Uri?> GetFilePathDialog(TopLevel? topLevel)
    {
        // Get a reference to the TopLevel/window where this control is hosted
        if (topLevel == null)
            return null;

        var provider = topLevel.StorageProvider;

        var options = new FilePickerOpenOptions
        {
            Title = "Select a File",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("All")
                {
                    Patterns = ["*.mp3", "*.wav"]
                }
            ]
        };

        var result = await provider.OpenFilePickerAsync(options);
        App.Logger!.LogTrace("Asked for a file.");

        if (result.Count <= 0) return null;
        var file = result[0];
        var filePath = file.Path;
        App.Logger!.LogDebug("Got {FilePath} as file.", filePath);

        return filePath;
    }
}