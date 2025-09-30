using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TLikeArtist
{
    public string Id { get; set; } = null!;

    public string? Name { get; set; }

    public string? CoverUri { get; set; }

    public int? TrackCount { get; set; }

    public string? Genres { get; set; }
}
