using System;

namespace ConnectFour;

/// EventArgs container for game-end data. This is passed to event subscribers
/// The data object that describes what happened when the game ended.
public sealed class GameEndedEvent : EventArgs
{
    public CellState Winner { get; init; } //The winning player, or Empty if the game ended in a draw.
    
    public bool IsDraw { get; init; } // True if the game ended with all cells filled and no winner (draw).
    
    public int MoveCount { get; init; } // Total number of moves played before the game ended.
}

