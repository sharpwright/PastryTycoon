using System;
using PastryTycoon.Common.Commands;
using Orleans;
using PastryTycoon.Common.Dto;

namespace PastryTycoon.Common.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task InitializeGameStateAsync(InitializeGameStateCommand command);
    Task UpdateGameAsync(UpdateGameCommand command);
    Task<GameStatisticsDto> GetGameStatisticsAsync(Guid gameId);
}
