using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Meio.Api.Services;

public record CoverArtArchiveResponse(
    [property: JsonPropertyName("images")] List<CoverArtImage> Images
);

public record CoverArtImage(
    [property: JsonPropertyName("image")] string ImageUrl
);