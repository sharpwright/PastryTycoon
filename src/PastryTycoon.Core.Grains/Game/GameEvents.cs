using System;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Base class for all game-related events.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
[Alias("GameEvent")]
[GenerateSerializer]
public record GameEvent(
    string GameId
);

/// <summary>
/// Event that indicates the game state has been initialized.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="PlayerId">The unique identifier of the player</param>
/// <param name="RecipeIds">The list of recipe IDs available in the game</param>
/// <param name="StartTimeUtc">The start time of the game in UTC</param>
[Alias("GameStateInitializedEvent")]
[GenerateSerializer]
public record GameStateInitializedEvent(
    string GameId,
    string PlayerId,
    IReadOnlyList<string> RecipeIds,
    DateTime StartTimeUtc
) : GameEvent(GameId);

/// <summary>
/// Event that indicates the game state has been updated.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="UpdateTimeUtc">>The time when the game state was last updated in UTC</param>
[Alias("GameUpdatedEvent")]
[GenerateSerializer]
public record GameUpdatedEvent(
    string GameId,
    DateTime UpdateTimeUtc
) : GameEvent(GameId);

/// <summary>
/// Event that indicates a new recipe has been added to the game.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="RecipeId">>The unique identifier of the recipe that was added</param>
[Alias("RecipeAddedEvent")]
[GenerateSerializer]
public record RecipeAddedEvent(
    string GameId,
    string RecipeId
) : GameEvent(GameId);