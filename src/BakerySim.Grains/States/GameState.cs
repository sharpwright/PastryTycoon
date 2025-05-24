using BakerySim.Grains.Events;

namespace BakerySim.Grains.States;

[GenerateSerializer]
public class GameState
{
    [Id(0)] public Guid GameId { get; set; }
    [Id(1)] public string GameName { get; set; } = string.Empty;
    [Id(2)] public DateTime StartTime { get; set; }
    [Id(3)] public DateTime EndTime { get; set; }
    [Id(4)] public DateTime LastUpdatedAtTime { get; set; }

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
 