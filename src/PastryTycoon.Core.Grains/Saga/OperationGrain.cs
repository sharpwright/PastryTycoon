using System;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Saga;

namespace PastryTycoon.Core.Grains.Saga;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_OPERATION_EVENTS)]
[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS)]
public class OperationGrain : JournaledGrain<OperationState, OperationEvent>, IOperationGrain,
    IAsyncObserver<OperationSagaEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<OperationGrain> logger;

    public OperationGrain(ILogger<OperationGrain> logger)
    {
        this.logger = logger;
    }

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
                this.logger.LogInformation("OperationGrain handling OperationPendingEvent for OperationId: {OperationId}", e.OperationId);
                this.logger.LogInformation("OperationGrain primary key: {PrimaryKey}", this.GetPrimaryKey().ToString());
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


    public Task OnErrorAsync(Exception ex)
    {
        this.logger.LogError(ex, "OperationGrain encountered an error while processing events.");
        throw new NotImplementedException();
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<OperationSagaEvent>();
        await handle.ResumeAsync(this);
    }
}
