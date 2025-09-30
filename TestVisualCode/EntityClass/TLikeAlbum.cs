using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikeAlbum
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public string? Type { get; set; }

    public string? AlbumVersion { get; set; }

    public string? Year { get; set; }

    public string? Genre { get; set; }

    public string? CoverUri { get; set; }

    public int? TracksCount { get; set; }
}
