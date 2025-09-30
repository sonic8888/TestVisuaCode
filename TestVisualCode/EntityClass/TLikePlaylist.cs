using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikePlaylist
{
    public int AutoId { get; set; }

    public string OwnerUid { get; set; } = null!;

    public string? OwnerName { get; set; }

    public string Kind { get; set; } = null!;

    public int? Revision { get; set; }

    public int? TrackCount { get; set; }

    public string? Title { get; set; }

    public string? CoverUri { get; set; }

    public string? PlaylistUuid { get; set; }
}
