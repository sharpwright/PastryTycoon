using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

/// <summary>
/// Defines commands related to game management in the Pastry Tycoon application.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="PlayerId">The unique identifier of the player</param>
/// <param name="RecipeIds">The list of recipe identifiers that are discoverable in the game</param>
/// <param name="StartTimeUtc">The UTC timestamp when the game starts</param>
[GenerateSerializer]
public record InitializeGameStateCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] List<string> RecipeIds,
    [property: Id(4)] DateTime StartTimeUtc
);

/// <summary>
/// Command to update the game state in the Pastry Tycoon application.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="UpdateTimeUtc">The UTC timestamp when the game was last updated</param>
[GenerateSerializer]
public record UpdateGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(2)] DateTime UpdateTimeUtc
);

/// <summary>
/// Command to add a recipe to a game in the Pastry Tycoon application.
/// </summary>
/// <param name="GameId">The unique identifier of the game</param>
/// <param name="RecipeId">The unique identifier of the recipe to be added</param>
[GenerateSerializer]
public record AddRecipeToGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid RecipeId
);