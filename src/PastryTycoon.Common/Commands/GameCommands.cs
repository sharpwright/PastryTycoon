using Orleans;

namespace PastryTycoon.Common.Commands;

[GenerateSerializer]
public record InitializeGameStateCommand(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] List<Guid> RecipeIds,
    [property: Id(3)] string GameName,
    [property: Id(4)] DateTime StartTimeUtc
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