using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TPlaylist
{
    public int AutoId { get; set; }

    public string Kind { get; set; } = null!;

    public string? PlaulistUuid { get; set; }

    public int? Revision { get; set; }

    public string? IdForFrom { get; set; }

    public int? TrackCount { get; set; }

    public string? Title { get; set; }

    public string? CoverUri { get; set; }

    public int? Status { get; set; }

    public int? DurationMillis { get; set; }

    public int? IsCollective { get; set; }

    public string? Description { get; set; }

    public string? DescriptionFormatted { get; set; }
}
