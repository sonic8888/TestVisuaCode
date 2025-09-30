using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikeEntityDownloadedTrack
{
    public int AutoId { get; set; }

    public string LikeEntityId { get; set; } = null!;

    public string TrackId { get; set; } = null!;

    public string? AlbumId { get; set; }
}
