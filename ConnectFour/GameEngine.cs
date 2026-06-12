using System;
using System.Linq;

namespace ConnectFour;

/// Core game engine: manages board state and game rules.
public sealed class GameEngine(IPlayer player1, IPlayer player2)
{
    public const int Rows = 6;
    public const int Columns = 7;
    
    private readonly CellState[][] _board = CreateEmptyBoard();
    
    /// Four unique directions to scan for a 4-in-a-row win:
    private static readonly (int deltaRow, int deltaColumn)[] Directions = [ (0, 1), (1, 0), (1, 1), (1, -1) ];

    /// Fired when the game ends (win or draw).
    public event EventHandler<GameEndedEvent>? OnGameEnded;
    
    public CellState CurrentPlayer { get; private set; } = CellState.Player1;
    public int MoveCount { get; private set; } = 0; //Total moves played so far in this game
    
    /// Runs the game loop until win or draw.
    public void Run()
    {
        var gameOver = false;

        do
        {
            DisplayBoard();
            var activePlayer = CurrentPlayer == CellState.Player1 ? player1 : player2;

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
    private void SwitchPlayer() => CurrentPlayer = CurrentPlayer == CellState.Player1 ? CellState.Player2 : CellState.Player1;
    
    /// Checks if the most recently placed piece (at row, col) forms a 4-in-a-row
    /// in any of the four directions.
    private bool CheckWin(int row, int col) =>
        Directions.Any(d => 1
            + CountInDirection(row, col, d.deltaRow, d.deltaColumn)
            + CountInDirection(row, col, -d.deltaRow, -d.deltaColumn) >= 4);
    
    /// Counts how many consecutive same pieces in a given direction
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
    private bool IsDraw() => MoveCount >= Rows * Columns;

    private void EndGame(CellState winner, bool isDraw) =>
        OnGameEnded?.Invoke(this, new GameEndedEvent(winner, isDraw, MoveCount));
    
    /// Prints the current board state to the console with column numbers.
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
    private static CellState[][] CreateEmptyBoard() =>
        Enumerable.Range(0, Rows).Select(_ => new CellState[Columns]).ToArray();
    
    /// Converts a CellState to its display character: X, O, or .
    private static string ToSymbol(CellState state) => state switch
    {
        CellState.Player1 => "X",
        CellState.Player2 => "O",
        _ => "."
    };
}
