using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TTrackLyric
{
    public string TrackId { get; set; } = null!;

    public string? Lyrics { get; set; }

    public string? FullLyrics { get; set; }

    public string? Url { get; set; }

    public int? HasRights { get; set; }
}
