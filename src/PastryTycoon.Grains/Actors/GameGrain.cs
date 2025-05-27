using System;
using PastryTycoon.Common.Constants;
using PastryTycoon.Common.Commands;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Common.Actors;
using PastryTycoon.Grains.States;
using PastryTycoon.Grains.Events;
using Orleans.EventSourcing;
using PastryTycoon.Common.Dto;
using PastryTycoon.Grains.Validation;
using FluentValidation;
using PastryTycoon.Grains.CommandHandlers;

namespace PastryTycoon.Grains.Actors;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS)]
public class GameGrain : JournaledGrain<GameState, GameEvent>, IGameGrain
{
    private IAsyncStream<GameEvent>? gameEventStream;
    private readonly InitializeGameCommandHandler initializeGameCommandHandler;

    public GameGrain(InitializeGameCommandHandler initializeGameStateCommand)
    {
        this.initializeGameCommandHandler = initializeGameStateCommand;
    }
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        gameEventStream = streamProvider.GetStream<GameEvent>(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    public async Task InitializeGameStateAsync(InitializeGameStateCommand command)
    {
        // Validate the command.
        var result = await initializeGameCommandHandler.HandleAsync(command, State, this.GetPrimaryKey());

        if (!result.IsSuccess)
        {
            throw new InvalidOperationException($"Failed to initialize game state: {result.ErrorMessage}");
        }

        var evt = result.GetEvent<GameEvent>();
        RaiseEvent(evt);
        await ConfirmEvents();

        if (gameEventStream != null)
        {
            await gameEventStream.OnNextAsync(evt);
        }        
    }

    public async Task UpdateGameAsync(UpdateGameCommand command)
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
    
    public Task<GameStatisticsDto> GetGameStatisticsAsync(Guid gameId)
    {
        if (!gameId.Equals(this.GetPrimaryKey()))
        {
            throw new ArgumentException("GameId does not match grain primary key.");
        }

        return Task.FromResult(new GameStatisticsDto
        {
            GameId = State.GameId,
            PlayerId = State.PlayerId,
            GameName = State.GameName,
            TotalRecipes = State.DiscoverableRecipeIds != null ? State.DiscoverableRecipeIds.Count : 0,
            StartTimeUtc = State.StartTimeUtc,
            LastUpdatedUtc = State.LastUpdatedAtTimeUtc
        });
    }
}
