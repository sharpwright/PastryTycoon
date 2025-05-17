using System;

namespace BakerySim.Grains.Events;

[GenerateSerializer]
public record GameStartedEvent(
    [property: Id(0)] Guid GameId,
    [property: Id(1)] string GameName,
    [property: Id(2)] DateTime StartTime
);
