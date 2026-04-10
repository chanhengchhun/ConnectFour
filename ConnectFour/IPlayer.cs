namespace ConnectFour;

/// Shared contract for all player types (human or AI).
public interface IPlayer
{
    string Name { get; }

    // Returns the chosen column index for the current turn.
    int GetMove(CellState[][] board);
}

