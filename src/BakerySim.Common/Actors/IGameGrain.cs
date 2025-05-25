using System;
using BakerySim.Common.Commands;
using Orleans;

namespace BakerySim.Common.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task StartGame(StartGameCommand command);
    Task UpdateGame(UpdateGameCommand command);
    Task AddAvailableRecipeAsync(AddRecipeToGameCommand command);
}
