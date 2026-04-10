using System;
using System.Collections.Generic;
using System.Linq;

namespace ConnectFour;

/// Chooses a random valid column.
public sealed class PlayerAI : IPlayer
{
    private readonly Random _random = new();

    public string Name { get; }

    public PlayerAI(string name)
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

