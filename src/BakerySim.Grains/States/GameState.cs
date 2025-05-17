namespace BakerySim.Grains.States;

[GenerateSerializer]
public class GameState
{
    [Id(0)] public Guid GameId { get; set; }
    [Id(1)] public string GameName { get; set; } = string.Empty;
    [Id(2)] public DateTime StartTime { get; set; }
    [Id(3)] public DateTime EndTime { get; set; }
}
 