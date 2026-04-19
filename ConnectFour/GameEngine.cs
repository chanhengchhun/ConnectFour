using System;

namespace ConnectFour; // file-scope name space

/// Core game engine: manages the board, enforces rules, detects wins/draws,
/// and orchestrates the turn-by-turn game loop between two players.
public sealed class GameEngine
{
    public const int Rows = 6;
    public const int Columns = 7;
    
    /// Four unique directions to scan for a 4-in-a-row win:
    /// (0,1) = horizontal right, (1,0) = vertical down,
    /// (1,1) = diagonal down-right, (1,-1) = diagonal down-left.
    private static readonly (int deltaRow, int deltaColumn)[] Directions =
    {
        (0, 1),
        (1, 0),
        (1, 1),
        (1, -1)
    };

    private readonly IPlayer _player1;
    private readonly IPlayer _player2;
    private CellState[][] _board;
    
    /// Initializes a new game with two players and an empty board.
    public GameEngine(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;
        _board = CreateEmptyBoard();
        
        CurrentPlayer = CellState.Player1;
        MoveCount = 0;
    }
    
    /// Fired when the game ends (win or draw). 
    /// Subscribers receive a GameEndedEvent with the winner, draw status, and total move count.
    public event EventHandler<GameEndedEvent>? OnGameEnded;
    
    public CellState CurrentPlayer { get; private set; } 
    public int MoveCount { get; private set; } //Total moves played so far in this game
    
    /// Main game loop: repeatedly displays the board, gets moves from each player,
    /// updates the board, checks for win/draw, and switches turns until game ends.
    public void Run()
    {
        var gameOver = false;

        do
        {
            DisplayBoard();
            var activePlayer = CurrentPlayer == CellState.Player1 ? _player1 : _player2;

            var col = activePlayer.GetMove(_board);
            var row = GetLandingRow(col);

            if (row == -1)
            {
                Console.WriteLine("That column is full. Try a different column.");
                continue;
            }

            PlacePiece(row, col, CurrentPlayer);

            if (CheckWin(row, col))
            {
                DisplayBoard();
                EndGame(CurrentPlayer, isDraw: false);
                gameOver = true;
            }
            else if (IsDraw())
            {
                DisplayBoard();
                EndGame(CellState.Empty, isDraw: true);
                gameOver = true;
            }
            else
            {
                SwitchPlayer();
            }
        } while (!gameOver);
    }
    
    /// Finds the lowest empty row in a column.
    /// Returns -1 if the column is full.
    private int GetLandingRow(int col)
    {
        // Scan upward from the bottom to model gravity in Connect Four.
        for (int row = Rows - 1; row >= 0; row--)
        {
            if (_board[row][col] == CellState.Empty)
            {
                return row;
            }
        }

        return -1;
    }
    
    /// Places a piece on the board at the specified position and increments move count.
    private void PlacePiece(int row, int col, CellState player)
    {
        _board[row][col] = player;
        MoveCount++;
    }
    
    /// Toggles between Player1 and Player2.
    private void SwitchPlayer() =>
        CurrentPlayer = CurrentPlayer == CellState.Player1 ? CellState.Player2 : CellState.Player1;
    
    /// Checks if the most recently placed piece (at row, col) forms a 4-in-a-row
    /// in any of the four directions.
    private bool CheckWin(int row, int col)
    {
        foreach (var direction in Directions)
        {
            // Count matching pieces in both opposite directions from the latest move.
            var connectedCount = 1
                                 + CountInDirection(row, col, direction.deltaRow, direction.deltaColumn)
                                 + CountInDirection(row, col, -direction.deltaRow, -direction.deltaColumn);

            if (connectedCount >= 4)
            {
                return true;
            }
        }

        return false;
    }
    
    /// Counts how many consecutive same-colored pieces lie in a given direction
    /// starting from (row, col). Stops at board edge or different piece color.
    private int CountInDirection(int row, int col, int dr, int dc)
    {
        var count = 0;
        var current = _board[row][col];
        // dr/dc describe the step vector: e.g., right (0,1), down (1,0), diagonal (1,1).
        var r = row + dr;
        var c = col + dc;

        while (r >= 0 && r < Rows && c >= 0 && c < Columns && _board[r][c] == current)
        {
            count++;
            r += dr;
            c += dc;
        }

        return count;
    }


    /// Checks if the board is completely full (all 42 cells occupied).
    private bool IsDraw()
    {
        return MoveCount >= Rows * Columns;
    }
    
    /// Fires the OnGameEnded event with the final game result.
    private void EndGame(CellState winner, bool isDraw)
    {
        OnGameEnded?.Invoke(this, new GameEndedEvent
        {
            Winner = winner,
            IsDraw = isDraw,
            MoveCount = MoveCount
        });
    }


    /// Prints the current board state to the console with column numbers.
    /// X represents Player1, O represents Player2, . represents Empty.
    private void DisplayBoard()
    {
        Console.WriteLine();
        Console.WriteLine("  0  1  2  3  4  5  6");

        for (var row = 0; row < Rows; row++)
        {
            for (var col = 0; col < Columns; col++)
            {
                Console.Write($"  {ToSymbol(_board[row][col])}");
            }

            Console.WriteLine();
        }

        Console.WriteLine();
    }
    
    /// Creates a 6x7 board with all cells initialized to Empty.
    private static CellState[][] CreateEmptyBoard()
    {
        // Build the board explicitly so each cell starts as Empty.
        var board = new CellState[Rows][];

        for (var row = 0; row < Rows; row++)
        {
            board[row] = new CellState[Columns];

            for (var col = 0; col < Columns; col++)
            {
                board[row][col] = CellState.Empty;
            }
        }

        return board;
    }
    
    /// Converts a CellState to its display character: X, O, or .
    private static string ToSymbol(CellState state)
    {
        switch (state)
        {
            case CellState.Player1:
                return "X";
            case CellState.Player2:
                return "O";
            default:
                return ".";
        }
    }
}

