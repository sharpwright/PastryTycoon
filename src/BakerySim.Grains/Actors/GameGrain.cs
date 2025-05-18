using System;
using BakerySim.Common.Orleans;
using BakerySim.Grains.Commands;
using BakerySim.Grains.Events;
using BakerySim.Grains.Projections;
using BakerySim.Grains.States;
using Orleans.Streams;

namespace BakerySim.Grains.Actors;

public class GameGrain : Grain, IGameGrain
{
    private readonly IPersistentState<GameState> state;
    private IAsyncStream<GameStartedEvent>? gameStartedStream;

    public GameGrain(
        [PersistentState("game", OrleansConstants.AZURE_TABLE_GRAIN_STORAGE)] IPersistentState<GameState> state)
    {
        this.state = state;
    }

    /// <summary>
    /// OnActivateAsync is called when the grain is activated and creates the stream to push events to.
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        // Gets the stream provider and creates the stream for game events.
        // NOTE: When creating a StreamId, ensure that the key is using the same type as the grain's identity.
        // For example, using this.GetPrimaryKey().ToString() will throw an ArgumentException on the streamId on runtime.
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var streamId = StreamId.Create(OrleansConstants.STREAM_GAME_NAMESPACE, this.GetPrimaryKey());
        gameStartedStream = streamProvider.GetStream<GameStartedEvent>(streamId);
        await base.OnActivateAsync(cancellationToken);

        // NOTE: This is an example of explicitly subscribing to a stream!
        // The stream is created but not yet subscribed to. The subscription will be handled by the ExplicitProjectionGrain grain.
        // We need to ensure activation of the ExplicitProjectionGrain to handle events.
        var projectionGrain = GrainFactory.GetGrain<IExplicitGameProjectionGrain>(this.GetPrimaryKey());
        await projectionGrain.EnsureActivated();
    }

    /// <summary>
    /// StartGame is called to start a game and pushes the GameStartedEvent to the stream.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    public async Task StartGame(StartGameCommand command)
    {
        state.State.GameId = command.GameId;
        state.State.GameName = command.GameName;
        state.State.StartTime = command.StartTime;
        await state.WriteStateAsync();

        if (gameStartedStream != null)
        {
            var evt = new GameStartedEvent(command.GameId, command.GameName, command.StartTime);
            await gameStartedStream.OnNextAsync(evt);
        }

        
    }
}
