using System.Collections.Immutable;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Represents the state of a game in the Pastry Tycoon application.
/// </summary>
[GenerateSerializer]
public class GameState
{
    [Id(0)] public Guid GameId { get; set; }
    [Id(1)] public Guid PlayerId { get; set; }
    [Id(2)] public IImmutableList<Guid>? DiscoverableRecipeIds { get; set; }
    [Id(3)] public DateTime StartTimeUtc { get; set; }
    [Id(4)] public DateTime LastUpdatedAtTimeUtc { get; set; }
    [Id(5)] public bool IsInitialized { get; set; } = false;
    
    /// <summary>
    /// Applies the GameStateInitializedEvent to initialize the game state.
    /// </summary>
    /// <param name="evt">The event containing the initialization details.</param>
    public void Apply(GameStateInitializedEvent evt)
    {
        GameId = evt.GameId;
        PlayerId = evt.PlayerId;
        DiscoverableRecipeIds = evt.RecipeIds.ToImmutableList();
        StartTimeUtc = evt.StartTimeUtc;
        LastUpdatedAtTimeUtc = evt.StartTimeUtc;
        IsInitialized = true;
    }

    /// <summary>
    /// Applies the RecipeAddedEvent to add a new recipe to the game state.
    /// </summary>
    /// <param name="evt">The event containing the recipe details.</param>
    public void Apply(GameUpdatedEvent evt)
    {
        LastUpdatedAtTimeUtc = evt.UpdateTimeUtc;
    }
}
 