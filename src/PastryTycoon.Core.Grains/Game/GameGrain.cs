using System;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.EventSourcing;
using PastryTycoon.Core.Grains.Game.Validators;
using PastryTycoon.Core.Abstractions.Game;
using PastryTycoon.Core.Grains.Common;
using PastryTycoon.Core.Abstractions.Common;
using PastryTycoon.Core.Grains.Game.CommandHandlers;

namespace PastryTycoon.Core.Grains.Game;

/// <summary>
/// Grain that represents a game instance in the Pastry Tycoon application.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_GAME_EVENTS)]
public class GameGrain : JournaledGrain<GameState, GameEvent>, IGameGrain
{
    private IAsyncStream<GameEvent>? gameEventStream;
    private readonly ICommandHandler<InitGameStateCmd, GameState, Guid, GameEvent> initGameHandler;
    private readonly ICommandHandler<UpdateGameCmd, GameState, Guid, GameEvent> updGameHandler;

    public GameGrain(
        ICommandHandler<InitGameStateCmd, GameState, Guid, GameEvent> initGameHandler,
        ICommandHandler<UpdateGameCmd, GameState, Guid, GameEvent> updGameHandler)
    {
        this.initGameHandler = initGameHandler;
        this.updGameHandler = updGameHandler;
    }
    
    /// <summary>
    /// Called when the grain is activated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for activation.</param>
    /// <returns></returns>
    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        var streamProvider = this.GetStreamProvider(OrleansConstants.STREAM_PROVIDER_NAME);
        gameEventStream = streamProvider.GetStream<GameEvent>(OrleansConstants.STREAM_NAMESPACE_GAME_EVENTS, this.GetPrimaryKey());
        await base.OnActivateAsync(cancellationToken);
    }

    /// <summary>
    /// Initializes the game state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the game initialization details.</param>
    /// <returns></returns>
    public async Task<CommandResult> InitializeGameStateAsync(InitGameStateCmd command)
    {
        // Validate the command.
        var handlerResult = await initGameHandler.HandleAsync(command, State, this.GetPrimaryKey());

        if (!handlerResult.IsSuccess)
        {
            // If the handler result indicates failure, return the errors
            return CommandResult.Failure([.. handlerResult.Errors]);
        }

        // If the command is valid, create the event to initialize the game state.
        if (handlerResult.IsSuccess && handlerResult.Event != default)
        {
            RaiseEvent(handlerResult.Event);
            await ConfirmEvents();

            if (gameEventStream != null
                && handlerResult.Event is GameStateInitializedEvent evt)
            {
                await gameEventStream.OnNextAsync(evt);
            }
        }

        return CommandResult.Success();
    }

    /// <summary>
    /// Updates the game state based on the provided command.
    /// </summary>
    /// <param name="command">The command containing the game update details.</param>
    /// <returns></returns>
    public async Task<CommandResult> UpdateGameAsync(UpdateGameCmd command)
    {
        var handlerResult = await updGameHandler.HandleAsync(command, State, this.GetPrimaryKey());

        if (!handlerResult.IsSuccess)
        {
            // If the handler result indicates failure, return the errors
            return CommandResult.Failure([.. handlerResult.Errors]);
        }

        // If the command is valid, create the event to initialize the game state.
        if (handlerResult.IsSuccess && handlerResult.Event != default)
        {
            RaiseEvent(handlerResult.Event);
            await ConfirmEvents();

            if (gameEventStream != null
                && handlerResult.Event is GameUpdatedEvent evt)
            {
                await gameEventStream.OnNextAsync(evt);
            }
        }
        
        return CommandResult.Success();
    }
    
    /// <summary>
    /// Retrieves the game statistics for the current game instance.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">Thrown if the game state is not initialized.</exception>
    public Task<GameStatisticsDto> GetGameStatisticsAsync()
    {
        if (!State.IsInitialized)
        {
            throw new InvalidOperationException("Game state is not initialized. Please initialize the game first.");
        }

        return Task.FromResult(new GameStatisticsDto(
            State.GameId,
            State.PlayerId,
            State.DiscoverableRecipeIds != null ? State.DiscoverableRecipeIds.Count : 0,
            State.StartTimeUtc,
            State.LastUpdatedAtTimeUtc));
    }
}
