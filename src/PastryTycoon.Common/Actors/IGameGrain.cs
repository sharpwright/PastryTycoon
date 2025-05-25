using System;
using PastryTycoon.Common.Commands;
using Orleans;

namespace PastryTycoon.Common.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task InitializeGameStateAsync(InitializeGameStateCommand command);
    Task UpdateGameAsync(UpdateGameCommand command);
}
