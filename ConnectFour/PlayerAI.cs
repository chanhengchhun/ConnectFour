using System;
using System.Collections.Generic;

namespace ConnectFour;

/// AI player that picks a random valid column each turn.
public sealed class PlayerAI : IPlayer
{
    private readonly Random _random = new();
    
    public string Name { get; }
    
    public PlayerAI(string name)
    {
        Name = name;
    }
    
    /// Returns a random valid column.
    public int GetMove(CellState[][] board)
    {
        var validColumns = GetValidColumns(board);
        return validColumns[_random.Next(validColumns.Count)];
    }
    
    /// Scans all columns and returns a list of those with an empty top cell
    private static List<int> GetValidColumns(CellState[][] board)
    {
        var validColumns = new List<int>();

        for (var column = 0; column < GameEngine.Columns; column++)
        {
            // If the top cell is empty, at least one piece can still be dropped in this column.
            if (board[0][column] == CellState.Empty)
            {
                validColumns.Add(column);
            }
        }

        return validColumns;
    }
}

