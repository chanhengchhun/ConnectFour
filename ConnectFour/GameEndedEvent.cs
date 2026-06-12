using System;

namespace ConnectFour;

public sealed class GameEndedEvent(CellState winner, bool isDraw, int moveCount) : EventArgs
{
    public readonly CellState Winner = winner;
    public readonly bool IsDraw = isDraw;
    public readonly int MoveCount = moveCount;
}
