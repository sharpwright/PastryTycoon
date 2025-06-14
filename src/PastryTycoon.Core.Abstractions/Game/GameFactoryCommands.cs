using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

/// <summary>
/// Represents the difficulty levels available for a game.
/// </summary>
public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

/// <summary>
/// Command to create a new game in the Pastry Tycoon application.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player creating the game.</param>
/// <param name="PlayerName">The name of the player creating the game.</param>
/// <param name="DifficultyLevel">The difficulty level of the game being created.</param>
[GenerateSerializer]
public record CreateNewGameCmd(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] DifficultyLevel DifficultyLevel
);