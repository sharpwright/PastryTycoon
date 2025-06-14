using System;

namespace PastryTycoon.Core.Grains.Player;

/// <summary>
/// Base class for all player-related events.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
[Alias("PlayerEvent")]
[GenerateSerializer]
public record PlayerEvent(
    Guid PlayerId
);

/// <summary>
/// Event that represents a player being initialized in the game.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="PlayerName">The name of the player.</param>
/// <param name="GameId">The unique identifier for the game the player is associated with.</param>
/// <param name="CreatedAtUtc">The UTC timestamp when the player was created.</param>
[Alias("PlayerInitializedEvent")]
[GenerateSerializer]
public record PlayerInitializedEvent (
    Guid PlayerId,
    string PlayerName,
    Guid GameId,
    DateTime CreatedAtUtc
) : PlayerEvent(PlayerId);

/// <summary>
/// Event that represents a player discovering a recipe.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="RecipeId">The unique identifier for the recipe that was discovered.</param>
/// <param name="DiscoveryTimeUtc">Represents the UTC timestamp when the recipe was discovered.</param>
[Alias("PlayerDiscoveredRecipeEvent")]
[GenerateSerializer]
public record PlayerDiscoveredRecipeEvent(
    Guid PlayerId,
    string RecipeId,
    DateTime DiscoveryTimeUtc
) : PlayerEvent(PlayerId);

/// <summary>
/// Event that represents a player unlocking an achievement.
/// </summary>
/// <param name="PlayerId">The unique identifier for the player.</param>
/// <param name="AchievementId">The unique identifier for the achievement that was unlocked.</param>
/// <param name="UnlockedAtUtc">Represents the UTC timestamp when the achievement was unlocked.</param>
[Alias("PlayerUnlockedAchievementEvent")]
[GenerateSerializer]
public record PlayerUnlockedAchievementEvent
(
    Guid PlayerId,
    string AchievementId,
    DateTime UnlockedAtUtc
) : PlayerEvent(PlayerId);