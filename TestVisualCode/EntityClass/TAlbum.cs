using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TAlbum
{
    public string Id { get; set; } = null!;

    public string? Title { get; set; }

    public string? ArtistsString { get; set; }

    public string? AlbumVersion { get; set; }

    public string? Year { get; set; }

    public string? GenreId { get; set; }

    public string? GenreTitle { get; set; }

    public string? CoverUri { get; set; }

    public int? TrackCount { get; set; }

    public string? AlbumOptions { get; set; }
}
