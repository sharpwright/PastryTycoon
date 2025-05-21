using System;

namespace BakerySim.Grains.Events;

[GenerateSerializer]
public record GameEvent(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName
);

[GenerateSerializer]
public record GameStartedEvent(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime StartTime
) : GameEvent(
    GameId,
    GameName
);

[GenerateSerializer]
public record GameUpdatedEvent(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime UpdateTime
) : GameEvent(
    GameId,
    GameName
);
