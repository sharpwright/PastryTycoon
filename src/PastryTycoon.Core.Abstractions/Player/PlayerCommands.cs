using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

/// <summary>
/// Command to initialize a player in the Pastry Tycoon application.
/// </summary>
/// <param name="PlayerName">The name of the player to be initialized.</param>
/// <param name="GameId">The unique identifier of the game associated with the player.</param>
[GenerateSerializer]
public record InitPlayerCmd(
    [property: Id(0)] string PlayerName,
    [property: Id(1)] Guid GameId
);

/// <summary>
/// Command to discover a new recipe in the Pastry Tycoon application.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player discovering the recipe.</param>
/// <param name="IngredientIds">A list of ingredient identifiers required for the recipe discovery.</param>
[GenerateSerializer]
public record TryDiscoverRecipeCmd(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] IList<string> IngredientIds
);

/// <summary>
/// Command to unlock an achievement for a player in the Pastry Tycoon application.
/// </summary>
/// <param name="PlayerId">The unique identifier of the player unlocking the achievement.</param>
/// <param name="AchievementId">The unique identifier of the achievement being unlocked.</param>
/// <param name="UnlockedAtUtc">The UTC timestamp when the achievement was unlocked.</param>
[GenerateSerializer]
public record UnlockAchievementCmd(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string AchievementId,
    [property: Id(2)] DateTime UnlockedAtUtc
);