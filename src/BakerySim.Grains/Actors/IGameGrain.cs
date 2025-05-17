using System;

namespace BakerySim.Grains.Actors;

public interface IGameGrain : IGrainWithGuidKey
{
    Task StartGame();
    Task StopGame();
    Task<bool> IsGameRunning();
}
