using System;
using BakerySim.Grains.Events;
using Orleans.Streams;

namespace BakerySim.Grains.Observers;

public interface IGameStartedEventObserver : IAsyncObserver<GameStartedEvent>
{

}
