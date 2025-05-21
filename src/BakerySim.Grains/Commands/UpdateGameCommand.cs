namespace BakerySim.Grains.Commands;

[GenerateSerializer]
public record UpdateGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime UpdateTime
);