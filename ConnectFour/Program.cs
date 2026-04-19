using System;

namespace ConnectFour;

public static class Program
{
    /// Main Program
    public static void Main(string[] args)
    {
        PrintMenu();
        var choice = Console.ReadLine()?.Trim();
        bool player2IsAi = choice == "2";

        var (player1, player2) = CreatePlayers(player2IsAi);
        var engine = new GameEngine(player1, player2);

        engine.OnGameEnded += HandleGameEnded;
        engine.Run();
    }
    
    /// Displays the game mode selection menu on the console.
    private static void PrintMenu()
    {
        Console.WriteLine("Connect Four");
        Console.WriteLine("------------");
        Console.WriteLine("1) Human vs Human");
        Console.WriteLine("2) Human vs AI");
        Console.Write("Choose mode (1-2): ");
    }


    /// Creates two player objects based on the game mode.
    private static (IPlayer Player1, IPlayer Player2) CreatePlayers(bool player2IsAi)
    {
        IPlayer player1 = new PlayerHuman("Player 1 (X)");
        IPlayer player2 = player2IsAi ? new PlayerAI("Computer (O)") : new PlayerHuman("Player 2 (O)");
        return (player1, player2);
    }
    
    /// Event handler called by GameEngine when the game ends.
    private static void HandleGameEnded(object? sender, GameEndedEvent gameEndedEvent) { PrintEndGameMessage(gameEndedEvent); }

    /// Prints the final result message: either announcing the winner or declaring a draw.
    private static void PrintEndGameMessage(GameEndedEvent gameEndedEvent)
    {
        if (gameEndedEvent.IsDraw)
        {
            Console.WriteLine($"Draw after {gameEndedEvent.MoveCount} moves.");
        }
        else
        {
            var winnerText = gameEndedEvent.Winner == CellState.Player1 ? "Player 1 (X)" : "Player 2 (O)";
            Console.WriteLine($"{winnerText} wins in {gameEndedEvent.MoveCount} moves!");
        }
    }
}
