using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikeAlbumArtist
{
    public int AutoId { get; set; }

    public string LikeAlbumId { get; set; } = null!;

    public string ArtistId { get; set; } = null!;

    public int? Various { get; set; }
}
