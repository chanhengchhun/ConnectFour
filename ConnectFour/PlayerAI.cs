using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectFour;

/// AI player that picks a random valid column each turn.
public sealed class PlayerAI(string name) : IPlayer
{
    private readonly Random _random = new();
    
    public string Name => name;
    
    /// Returns a random valid column.
    public int GetMove(CellState[][] board)
    {
        var validColumns = GetValidColumns(board);
        return validColumns[_random.Next(validColumns.Count)];
    }
    
    /// Scans all columns and returns a list of those with an empty top cell
    private static List<int> GetValidColumns(CellState[][] board) =>
        Enumerable.Range(0, GameEngine.Columns)
            .Where(col => board[0][col] == CellState.Empty)
            .ToList();
}
