using System;
using BakerySim.Grains.Events;
using Microsoft.Extensions.Logging;
using Orleans.Streams;

namespace BakerySim.Grains.Observers;

/// <summary>
/// GameStartedEventObserver is an observer that handles GameStartedEvent.
/// </summary>
public class GameStartedEventObserver : IGameStartedEventObserver
{
    private readonly ILogger<GameStartedEventObserver> logger;

    public GameStartedEventObserver(ILogger<GameStartedEventObserver> logger)
    {
        this.logger = logger;
    }

    public Task OnNextAsync(GameStartedEvent item, StreamSequenceToken token = null)
    {
        // Handle the event here
        logger.LogInformation($"Game started at {item.StartTime}");
        return Task.CompletedTask;
    }

    public Task OnCompletedAsync()
    {
        // Handle completion
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        // Handle error
        logger.LogError(ex, $"Error: {ex.Message}");
        return Task.CompletedTask;
    }
}
