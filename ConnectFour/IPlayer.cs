namespace ConnectFour;

/// Shared contract (interface) for any player in the game. Both human and AI players
/// must implement this interface to participate in the game engine.
public interface IPlayer
{
    // players' name
    string Name { get; }
    
    // players' moves
    int GetMove(CellState[][] board);
}

