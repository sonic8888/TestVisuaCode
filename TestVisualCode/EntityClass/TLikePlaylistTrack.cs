using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikePlaylistTrack
{
    public int AutoId { get; set; }

    public string LikePlaylistId { get; set; } = null!;

    public string TrackId { get; set; } = null!;

    public string? AlbumId { get; set; }
}
