using System;

namespace BakerySim.Grains.Actors;

public class GameGrain : Grain, IGameGrain
{
    public Task<bool> IsGameRunning()
    {
        throw new NotImplementedException();
    }

    public Task StartGame()
    {
        throw new NotImplementedException();
    }

    public Task StopGame()
    {
        throw new NotImplementedException();
    }
}
