using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TPlaybackQueue
{
    public string? QueueType { get; set; }

    public string? RemoteId { get; set; }

    public int? IsShuffled { get; set; }

    public int? TrackPosition { get; set; }

    public string? StationId { get; set; }
}
