using System;

namespace ConnectFour;
public static class Program
{
    public static void Main(string[] args)
    {
        PrintMenu();
        var choice = Console.ReadLine()?.Trim();
        var player2IsAi = choice == "2";

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
        IPlayer player1 = new PlayerHuman("Player 1 (X)");
        IPlayer player2 = player2IsAi ? new PlayerAI("Computer (O)") : new PlayerHuman("Player 2 (O)");
        return (player1, player2);
    }

    private static void PrintEndGameMessage(GameEndedEvent @event)
    {
        if (@event.IsDraw)
        {
            Console.WriteLine($"Draw after {@event.MoveCount} moves.");
        }
        else
        {
            var winnerText = @event.Winner == CellState.Player1 ? "Player 1 (X)" : "Player 2 (O)";
            Console.WriteLine($"{winnerText} wins in {@event.MoveCount} moves!");
        }
    }
}
