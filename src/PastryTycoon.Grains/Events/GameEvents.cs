using System;

namespace PastryTycoon.Grains.Events;

[GenerateSerializer]
public record GameEvent(
    [property: Id(0)] Guid GameId
);

[GenerateSerializer]
public record GameStateInitializedEvent(
    Guid GameId,
    [property: Id(1)] Guid PlayerId,
    [property: Id(2)] IReadOnlyList<Guid> RecipeIds,
    [property: Id(3)] string GameName,
    [property: Id(4)] DateTime StartTimeUtc
) : GameEvent(
    GameId
);

[GenerateSerializer]
public record GameUpdatedEvent(
    Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime UpdateTimeUtc
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