using System;
using System.Linq;
using System.Collections.Generic;

namespace ConnectFour
{
    /// Represents one slot on the board. (empty or occupied)
    public enum CellState { Empty, Player1, Player2 }

    /// Shared contract for any player implementation.
    public interface IPlayer
    {
        string Name { get; }
        int GetMove(CellState[][] board);
    }

    /// Data sent when a game finishes.
    public sealed class GameEndedEventArgs : EventArgs
    {
        public CellState Winner { get; init; }
        public bool IsDraw { get; init; }
        public int MoveCount { get; init; }
    }

    /// Gets moves from console input.
    public sealed class HumanPlayer : IPlayer
    {
        public string Name { get; }

        public HumanPlayer(string name)
        {
            Name = name;
        }

        public int GetMove(CellState[][] board)
        {
            while (true)
            {
                Console.Write($"{Name}, enter a column (0-6): ");
                var raw = Console.ReadLine();
                var isInt = int.TryParse(raw, out var column);

                if (isInt && column is >= 0 and <= 6)
                {
                    return column;
                }

                Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
            }
        }
    }

    /// Picks a random valid column.
    public sealed class AIPlayer : IPlayer
    {
        private readonly Random _random = new();
        public string Name { get; }

        public AIPlayer(string name)
        {
            Name = name;
        }

        public int GetMove(CellState[][] board)
        {
            var validColumns = GetValidColumns(board);
            return validColumns[_random.Next(validColumns.Count)];
        }

        private static List<int> GetValidColumns(CellState[][] board)
        {
            return Enumerable.Range(0, GameEngine.Columns)
                .Where(column => board[0][column] == CellState.Empty)
                .ToList();
        }
    }

    /// Core game loop and rules.
    public sealed class GameEngine
    {
        public const int Rows = 6;
        public const int Columns = 7;

        private static readonly (int deltaRow, int deltaColumn)[] Directions = { (0, 1), (1, 0), (1, 1), (1, -1) };

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

        public event EventHandler<GameEndedEventArgs>? OnGameEnded;

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
                else { SwitchPlayer(); }
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

        private void SwitchPlayer() => CurrentPlayer = CurrentPlayer == CellState.Player1 ? CellState.Player2 : CellState.Player1;

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

        private bool IsDraw() => MoveCount >= Rows * Columns;

        private void EndGame(CellState winner, bool isDraw)
        {
            OnGameEnded?.Invoke(this, new GameEndedEventArgs
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
                    return "x";
            }
        }
    }

    /// App entry point.
    public static class Program
    {
        public static void Main(string[] args)
        {
            PrintMenu();
            var choice = Console.ReadLine()?.Trim();
            bool player2IsAi = choice == "2";
            var (player1, player2) = CreatePlayers(player2IsAi);
            var engine = new GameEngine(player1, player2);
            engine.OnGameEnded += (_, eventArgs) => PrintEndGameMessage(eventArgs);
            engine.Run();
        }

        private static void PrintMenu()
        {
            Console.WriteLine("Connect Four");
            Console.WriteLine("------------");
            Console.WriteLine("1) Human vs Human");
            Console.WriteLine("2) Human vs AI");
            Console.Write("Choose mode (1-2): ");
        }

        private static (IPlayer Player1, IPlayer Player2) CreatePlayers(bool player2IsAi)
        {
            IPlayer player1 = new HumanPlayer("Player 1 (X)");
            IPlayer player2 = player2IsAi ? new AIPlayer("Computer (O)") : new HumanPlayer("Player 2 (O)");
            return (player1, player2);
        }

        private static void PrintEndGameMessage(GameEndedEventArgs eventArgs)
        {
            if (eventArgs.IsDraw)
            {
                Console.WriteLine($"Draw after {eventArgs.MoveCount} moves.");
            }
            else
            {
                var winnerText = eventArgs.Winner == CellState.Player1 ? "Player 1 (X)" : "Player 2 (O)";
                Console.WriteLine($"{winnerText} wins in {eventArgs.MoveCount} moves!");
            }
        }
    }
}
