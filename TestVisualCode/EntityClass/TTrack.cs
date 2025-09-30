using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TTrack
{
    public string Id { get; set; } = null!;

    public string? RealId { get; set; }

    public string? Title { get; set; }

    public int? DurationMillis { get; set; }

    public int? Available { get; set; }

    public int? FileSize { get; set; }

    public string? Token { get; set; }

    public int? IsOffline { get; set; }

    public string? CoverUri { get; set; }

    public string? ContentWarning { get; set; }

    public int? IsLyricsAvailable { get; set; }

    public string? Type { get; set; }

    public string? TrackOptions { get; set; }

    public string? PubDate { get; set; }
}
