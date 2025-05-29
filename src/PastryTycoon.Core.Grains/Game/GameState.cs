using System.Collections.Immutable;

namespace PastryTycoon.Core.Grains.Game;

[GenerateSerializer]
public class GameState
{
    [Id(0)] public Guid GameId { get; set; }
    [Id(1)] public Guid PlayerId { get; set; }
    [Id(2)] public IImmutableList<Guid>? DiscoverableRecipeIds { get; set; }
    [Id(3)] public string GameName { get; set; } = string.Empty;
    [Id(4)] public DateTime StartTimeUtc { get; set; }
    [Id(5)] public DateTime LastUpdatedAtTimeUtc { get; set; }

    // Event sourcing: apply GameStartedEvent
    public void Apply(GameStateInitializedEvent evt)
    {
        GameId = evt.GameId;
        PlayerId = evt.PlayerId;
        DiscoverableRecipeIds = evt.RecipeIds.ToImmutableList();
        GameName = evt.GameName;
        StartTimeUtc = evt.StartTimeUtc;
        LastUpdatedAtTimeUtc = evt.StartTimeUtc;
    }

    // Event sourcing: apply GameUpdatedEvent
    public void Apply(GameUpdatedEvent evt)
    {
        GameName = evt.GameName;
        LastUpdatedAtTimeUtc = evt.UpdateTimeUtc;
    }
}
 