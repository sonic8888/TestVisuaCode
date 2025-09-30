using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikeAlbumTrack
{
    public int AutoId { get; set; }

    public string LikeAlbumId { get; set; } = null!;

    public string TrackId { get; set; } = null!;

    public string? AlbumId { get; set; }
}
