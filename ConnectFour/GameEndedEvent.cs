using System;

namespace ConnectFour;

/// Event data raised when a game ends.
public sealed class GameEndedEvent : EventArgs
{
    public CellState Winner { get; init; }
    public bool IsDraw { get; init; }
    public int MoveCount { get; init; }
}
