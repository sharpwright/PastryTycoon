using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

[GenerateSerializer]
public record CreateNewGameCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] string PlayerName,
    [property: Id(2)] DifficultyLevel DifficultyLevel
);