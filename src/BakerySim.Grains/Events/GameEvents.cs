using System;

namespace BakerySim.Common.Events;

[GenerateSerializer]
public record GameEvent(
    [property: Id(0)] Guid GameId
);

[GenerateSerializer]
public record GameStartedEvent(
    Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime StartTime
) : GameEvent(
    GameId
);

[GenerateSerializer]
public record GameUpdatedEvent(
    Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime UpdateTime
) : GameEvent(
    GameId
);

[GenerateSerializer]
public record RecipeAddedEvent(
    Guid GameId,
    [property: Id(2)] Guid RecipeId
) : GameEvent(
    GameId
);