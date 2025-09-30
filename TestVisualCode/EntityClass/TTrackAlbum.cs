using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TTrackAlbum
{
    public int AutoId { get; set; }

    public string TrackId { get; set; } = null!;

    public string AlbumId { get; set; } = null!;

    public int? TrackPosition { get; set; }

    public int? AlbumVolume { get; set; }
}
