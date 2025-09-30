using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TPlaylistTrack
{
    public int AutoId { get; set; }

    public string Kind { get; set; } = null!;

    public string TrackId { get; set; } = null!;

    public string? AlbumId { get; set; }

    public int? Status { get; set; }

    public long? Timestamp { get; set; }
}
