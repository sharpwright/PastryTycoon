using System;

namespace BakerySim.Grains.Actors;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
    Task<Guid> CreateNewGameAsync(Guid playerId, string gameName);
}
