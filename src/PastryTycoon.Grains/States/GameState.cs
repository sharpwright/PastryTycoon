using PastryTycoon.Common.Events;

namespace PastryTycoon.Common.States;

[GenerateSerializer]
public class GameState
{
    [Id(0)] public Guid GameId { get; set; }
    [Id(1)] public Guid PlayerId { get; set; }
    [Id(2)] public List<Guid> RecipeIds { get; set; } = new List<Guid>();
    [Id(3)] public string GameName { get; set; } = string.Empty;
    [Id(4)] public DateTime StartTime { get; set; }
    [Id(5)] public DateTime LastUpdatedAtTime { get; set; }

    // Event sourcing: apply GameStartedEvent
    public void Apply(GameStartedEvent evt)
    {
        GameId = evt.GameId;
        GameName = evt.GameName;
        StartTime = evt.StartTime;
        LastUpdatedAtTime = evt.StartTime;
    }

    // Event sourcing: apply GameUpdatedEvent
    public void Apply(GameUpdatedEvent evt)
    {
        GameName = evt.GameName;
        LastUpdatedAtTime = evt.UpdateTime;
    }
}
 