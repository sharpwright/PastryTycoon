using System;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Saga;

namespace PastryTycoon.Core.Grains.Saga;

/// <summary>
/// Grain that represents an operation in the operation saga.
/// It listens for events related to operations and handles them accordingly.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_OPERATION_EVENTS)]
[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS)]
public class OperationGrain : JournaledGrain<OperationState, OperationEvent>, IOperationGrain,
    IAsyncObserver<OperationSagaEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<OperationGrain> logger;

    /// <summary>
    /// Constructor for the OperationGrain.
    /// Initializes the grain with a logger for logging purposes.
    /// </summary>
    /// <param name="logger"></param>
    public OperationGrain(ILogger<OperationGrain> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Adds an operation to the grain's state.
    /// This method raises an event that indicates an operation has been added.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddOperation(AddOperationCommand command)
    {
        this.logger.LogInformation("OperationGrain adding operation with ID: {OperationId}", command.OperationId);

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var evt = new OperationAddedEvent(
            command.OperationId,
            command.Name
        );

        RaiseEvent(evt);
        await ConfirmEvents();
    }

    /// <summary>
    /// Handles incoming events from the operation saga stream.
    /// This method processes the event and performs actions based on the event type.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task OnNextAsync(OperationSagaEvent item, StreamSequenceToken? token = null)
    {
        this.logger.LogInformation(
            "OperationGrain (primary key: {PrimaryKey}) received event: {EventType} for OperationSagaId: {OperationSagaId}",
            this.GetPrimaryKey(),
            item.GetType().Name,
            item.OperationSagaId);

        switch (item)
        {
            case OperationPendingEvent e:
                // Handle the OperationPendingEvent by adding the operation
                this.logger.LogInformation("OperationGrain handling OperationPendingEvent for OperationId: {OperationId}", e.OperationId);
                this.logger.LogInformation("OperationGrain primary key: {PrimaryKey}", this.GetPrimaryKey().ToString());

                // Create a command to add the operation to ensure all validations and business logic are applied.
                var command = new AddOperationCommand(
                    e.OperationId,
                    e.Name
                );
                await AddOperation(command);
                break;
            default:
                this.logger.LogWarning(
                    "OperationGrain unhandled event type: {EventType} for OperationSagaId: {OperationSagaId}",
                    item.GetType().Name,
                    item.OperationSagaId);
                return;
        }
    }

    /// <summary>
    /// Handles errors that occur during event processing.
    /// This method is called when an error occurs while processing events.
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task OnErrorAsync(Exception ex)
    {
        this.logger.LogError(ex, "OperationGrain encountered an error while processing events.");
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handles the subscription to the operation saga stream.
    /// This method is called when the grain subscribes to the stream of operation saga events.
    /// </summary>
    /// <param name="handleFactory"></param>
    /// <returns></returns>
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<OperationSagaEvent>();
        await handle.ResumeAsync(this);
    }
}
