namespace ConnectFour;

/// <summary>
/// Represents the state of a single cell on the Connect Four board.
/// </summary>
public enum CellState
{
    /// <summary>No piece in this cell.</summary>
    Empty,
    /// <summary>Player 1's piece (displayed as 'X').</summary>
    Player1,
    /// <summary>Player 2's piece (displayed as 'O').</summary>
    Player2
}

