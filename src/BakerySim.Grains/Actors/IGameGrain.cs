using System;
using BakerySim.Grains.Commands;

namespace BakerySim.Grains.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task StartGame(StartGameCommand command);
    Task UpdateGame(UpdateGameCommand command);
    Task AddAvailableRecipeAsync(AddRecipeToGameCommand command);
}
