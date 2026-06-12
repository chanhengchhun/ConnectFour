using System;

namespace ConnectFour;

/// Human player that reads moves from console input.
public sealed class PlayerHuman(string name) : IPlayer
{
    public string Name => name;
    
    /// Prompts the player via console to enter a column (0-6).
    public int GetMove(CellState[][] board)
    {
        while (true)
        {
            Console.Write($"{Name}, enter a column (0-6): ");
            if (int.TryParse(Console.ReadLine(), out var column) && column is >= 0 and <= 6)
            {
                return column;
            }

            Console.WriteLine("Invalid input. Please enter a number between 0 and 6.");
        }
    }
}
