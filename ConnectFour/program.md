### C# Concepts to explain in ConnectFour Program

- **Interface `IPlayer`**: One contract, many implementations (`HumanPlayer`, `AIPlayer`) using polymorphism.
- **Event + EventHandler `OnGameEnded`**: `GameEngine` publishes game-finished info without hard-coding output logic.
- **Enum `CellState`**: Strong, readable board values (`Empty`, `Player1`, `Player2`) instead of magic numbers.
- **Tuple return `CreatePlayers`**: Returns two related values with names: `(Player1, Player2)`.
- **Init-only properties `get; init;`**: `GameEndedEventArgs` can only be set at creation, ensuring immutability after creations.
- **Lambda expressions `=>`**: Concise syntax for simple methods and event handlers.
- **LINQ `Range`, `Where`, `Select`, `ToList`**: Declarative filtering/projection for valid columns and board creation.
- **File-scoped namespace**: C# style, dotnet 10 and above, with less indentation.
```c# 
namespace ConnectFour;
```
is an equivalent to
```C#
namespace ConnectFour
{
    // code here
}
```
- **Nullable-aware event invoke `?.Invoke`**: Safely raises event only when listeners are attached.

### Quick talking points

- The project shows how C# separates **game rules** (`GameEngine`) from **player behavior** (`IPlayer` implementations).
- It demonstrates both **OOP** (classes, interface) and **functional style** (LINQ, lambdas) together.
- It uses language features that improve safety/readability without making code too advanced.

