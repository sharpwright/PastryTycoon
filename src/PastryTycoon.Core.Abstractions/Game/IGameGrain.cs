using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Game;

public interface IGameGrain : IGrainWithGuidKey
{
    Task InitializeGameStateAsync(InitializeGameStateCommand command);
    Task UpdateGameAsync(UpdateGameCommand command);
    Task<GameStatisticsDto> GetGameStatisticsAsync();
}
