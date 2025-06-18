using System;

namespace AuroraControls;

/// <summary>
/// Event arguments for when a chip is tapped in a ChipGroup.
/// </summary>
public class ChipTappedEventArgs : EventArgs
{
    /// <summary>
    /// Gets the chip that was tapped.
    /// </summary>
    public required Chip Chip { get; init; }
}
