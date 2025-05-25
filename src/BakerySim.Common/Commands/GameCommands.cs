using Orleans;

namespace BakerySim.Common.Commands;

[GenerateSerializer]
public record StartGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] string GameName,
    [property: Id(3)] DateTime StartTimeUtc
);

[GenerateSerializer]
public record UpdateGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime UpdateTimeUtc
);

[GenerateSerializer]
public record AddRecipeToGameCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid RecipeId
);