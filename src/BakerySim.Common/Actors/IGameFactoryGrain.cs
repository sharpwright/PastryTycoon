using System;
using Orleans;

namespace BakerySim.Common.Actors;

public interface IGameFactoryGrain : IGrainWithGuidKey
{
    Task<Guid> CreateNewGameAsync(Guid playerId, string gameName);
}
