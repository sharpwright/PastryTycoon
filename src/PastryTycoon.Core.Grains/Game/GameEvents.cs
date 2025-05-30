using System;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Base class for all game-related events.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
[GenerateSerializer]
public record GameEvent(
    [property: Id(0)] Guid GameId
);

/// <summary>
/// Event that indicates the game state has been initialized.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="PlayerId">The unique identifier of the player</param>
/// <param name="RecipeIds">The list of recipe IDs available in the game</param>
/// <param name="StartTimeUtc">The start time of the game in UTC</param>
[GenerateSerializer]
public record GameStateInitializedEvent(
    Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] IReadOnlyList<Guid> RecipeIds,
    [property: Id(4)] DateTime StartTimeUtc
) : GameEvent(GameId);

/// <summary>
/// Event that indicates the game state has been updated.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="UpdateTimeUtc">>The time when the game state was last updated in UTC</param>
[GenerateSerializer]
public record GameUpdatedEvent(
    Guid GameId,
    [property: Id(2)] DateTime UpdateTimeUtc
) : GameEvent(GameId);

/// <summary>
/// Event that indicates a new recipe has been added to the game.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="RecipeId">>The unique identifier of the recipe that was added</param>
[GenerateSerializer]
public record RecipeAddedEvent(
    Guid GameId,
    [property: Id(2)] Guid RecipeId
) : GameEvent(GameId);