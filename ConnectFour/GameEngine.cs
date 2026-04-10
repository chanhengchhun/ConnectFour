using System;
using System.Linq;

namespace ConnectFour;

/// Runs the main game loop and enforces game rules.
public sealed class GameEngine
{
    public const int Rows = 6;
    public const int Columns = 7;

    // Four unique line directions to scan for a 4-in-a-row.
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

    public GameEngine(IPlayer player1, IPlayer player2)
    {
        _player1 = player1;
        _player2 = player2;

        _board = CreateEmptyBoard();
        CurrentPlayer = CellState.Player1;
        MoveCount = 0;
    }

    public event EventHandler<GameEndedEvent>? OnGameEnded;

    public CellState CurrentPlayer { get; private set; }
    public int MoveCount { get; private set; }

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

    private int GetLandingRow(int col)
    {
        for (var row = Rows - 1; row >= 0; row--)
        {
            if (_board[row][col] == CellState.Empty)
            {
                return row;
            }
        }

        return -1;
    }

    private void PlacePiece(int row, int col, CellState player)
    {
        _board[row][col] = player;
        MoveCount++;
    }

    private void SwitchPlayer() =>
        CurrentPlayer = CurrentPlayer == CellState.Player1 ? CellState.Player2 : CellState.Player1;

    private bool CheckWin(int row, int col)
    {
        return Directions.Any(direction =>
            1 + CountInDirection(row, col, direction.deltaRow, direction.deltaColumn)
              + CountInDirection(row, col, -direction.deltaRow, -direction.deltaColumn) >= 4);
    }

    private int CountInDirection(int row, int col, int dr, int dc)
    {
        var count = 0;
        var current = _board[row][col];
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

    private bool IsDraw()
    {
        return MoveCount >= Rows * Columns;
    }

    private void EndGame(CellState winner, bool isDraw)
    {
        OnGameEnded?.Invoke(this, new GameEndedEvent
        {
            Winner = winner,
            IsDraw = isDraw,
            MoveCount = MoveCount
        });
    }

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

    private static CellState[][] CreateEmptyBoard()
    {
        return Enumerable
            .Range(0, Rows)
            .Select(_ => Enumerable.Repeat(CellState.Empty, Columns).ToArray())
            .ToArray();
    }

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

