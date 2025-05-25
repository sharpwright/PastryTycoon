using System;
using PastryTycoon.Common.Commands;
using Orleans;

namespace PastryTycoon.Common.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task StartGame(StartGameCommand command);
    Task UpdateGame(UpdateGameCommand command);
    Task AddAvailableRecipeAsync(AddRecipeToGameCommand command);
}
