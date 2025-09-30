using System;
using System.Collections.Generic;

namespace TestVisualCode;

public partial class TRadioSkip
{
    public string Id { get; set; } = null!;

    public int? SkipsSoFar { get; set; }

    public long? ValidThrough { get; set; }
}
