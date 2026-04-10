using System;

namespace ConnectFour;

/// Reads moves from console input.
public sealed class PlayerHuman : IPlayer
{
    public string Name { get; }

    public PlayerHuman(string name)
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
