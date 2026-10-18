using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meio.Api.Services;

public record CoverArtArchiveResponse(
    [property: JsonPropertyName("images")] List<CoverArtImage> Images
);

// ReSharper only checks for usage statically, however it is used as a JSON object for CoverArtArchiveResponse.
// ReSharper disable once ClassNeverInstantiated.Global
public record CoverArtImage(
    [property: JsonPropertyName("image")] string ImageUrl
);