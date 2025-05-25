using System;
using PastryTycoon.Common.Constants;
using PastryTycoon.Common.Commands;
using PastryTycoon.Common.Events;
using PastryTycoon.Common.States;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Common.Actors;

namespace PastryTycoon.Grains.Actors;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS)]
public class GameGrain : JournaledGrain<GameState, GameEvent>, IGameGrain
{
    private IAsyncStream<GameEvent>? gameEventStream;

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        gameEventStream = streamProvider.GetStream<GameEvent>(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task StartGame(StartGameCommand command)
    {
        if (!command.GameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("Command GameId does not match grain primary key.");
        }

        var evt = new GameStartedEvent(command.GameId, command.GameName, command.StartTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {            
            await gameEventStream.OnNextAsync(evt);
        }
    }

    public async Task UpdateGame(UpdateGameCommand command)
    {
        if (!command.GameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("Command GameId does not match grain primary key.");
        }

        var evt = new GameUpdatedEvent(command.GameId, command.GameName, command.UpdateTimeUtc);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {            
            await gameEventStream.OnNextAsync(evt);
        }
    }

    public async Task AddAvailableRecipeAsync(AddRecipeToGameCommand command)
    {
        if (!command.GameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("Command GameId does not match grain primary key.");
        }

        if (command.RecipeId == Guid.Empty)
        {
            throw new ArgumentException("RecipeId cannot be empty.");
        }

        var evt = new RecipeAddedEvent(command.GameId, command.RecipeId);
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {        
            await gameEventStream.OnNextAsync(evt);
        }    
    }
}
