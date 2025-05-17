namespace BakerySim.Grains.Commands;

[GenerateSerializer]
public record StartGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime StartTime
);