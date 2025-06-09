using System;

namespace PastryTycoon.Core.Grains.Player;

/// <summary>
/// Represents the state of a player in the game.
/// </summary>
[GenerateSerializer]
public class PlayerState
{
    [Id(0)] public Guid PlayerId { get; set; }
    [Id(1)] public string PlayerName { get; set; } = string.Empty;
    [Id(2)] public Guid GameId { get; set; } 
    [Id(3)] public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    [Id(4)] public DateTime LastActivityAtUtc { get; set; } = DateTime.UtcNow;
    [Id(5)] public bool IsInitialized { get; set; } = false;
    [Id(6)] public decimal Balance { get; set; } = 0.0m;    
    [Id(7)] public IDictionary<string, DateTime> CraftedRecipes { get; set; } = new Dictionary<string, DateTime>();
    [Id(8)] public IDictionary<string, DateTime> DiscoveredRecipes { get; set; } = new Dictionary<string, DateTime>();
    [Id(9)] public IDictionary<string, DateTime> UnlockedAchievements { get; set; } = new Dictionary<string, DateTime>();
    [Id(10)] public IDictionary<string, int> IngredientInventory { get; set; } = new Dictionary<string, int>();

    /// <summary>
    /// Applies the PlayerInitializedEvent to initialize the player state.
    /// </summary>
    /// <param name="evt">The event containing the initialization details.</param>
    public void Apply(PlayerInitializedEvent evt)
    {
        PlayerId = evt.PlayerId;
        PlayerName = evt.PlayerName;
        GameId = evt.GameId;
        CreatedAtUtc = evt.CreatedAtUtc;
        LastActivityAtUtc = DateTime.UtcNow;
        IsInitialized = true;
    }

    /// <summary>
    /// Applies the PlayerDiscoveredRecipeEvent to update the player's discovered recipes.
    /// </summary>
    /// <param name="evt">The event containing the recipe discovery details.</param>
    public void Apply(PlayerDiscoveredRecipeEvent evt)
    {
        DiscoveredRecipes[evt.RecipeId] = evt.DiscoveryTimeUtc;
        LastActivityAtUtc = DateTime.UtcNow;
    }

    /// <summary>
    /// Applies the PlayerUnlockedAchievementEvent to update the player's unlocked achievements.
    /// </summary>
    /// <param name="evt">The event containing the achievement unlock details.</param>
    public void Apply(PlayerUnlockedAchievementEvent evt)
    {
        UnlockedAchievements[evt.AchievementId] = evt.UnlockedAtUtc;
        LastActivityAtUtc = DateTime.UtcNow;
    }

}
