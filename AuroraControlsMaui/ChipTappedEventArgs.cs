using System;

namespace AuroraControls;

/// <summary>
/// Event arguments for when a chip is tapped in a ChipGroup.
/// </summary>
public class ChipTappedEventArgs : EventArgs
{
    public Chip Chip { get; private set; }

    public ChipState ChipState { get; private set; }

    public ChipTappedEventArgs(Chip chip)
    {
        Chip = chip;
        ChipState = chip.State;
    }
}
