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
        this.logger.LogInformation("Adding operation with ID: {OperationId}", command.OperationId);
        
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
        this.logger.LogInformation("OperationGrain received event: {EventType}", item.GetType().Name);

        switch (item)
        {
            case OperationPendingEvent e:
                await HandleOperationPending(e);
                break;
            default:
                this.logger.LogWarning("Unhandled event type: {EventType}", item.GetType().Name);
                return;
        }
    }


    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<OperationSagaEvent>();
        await handle.ResumeAsync(this);
    }

    private async Task HandleOperationPending(OperationPendingEvent e)
    {
        var operationAddedEvent = new OperationAddedEvent(
            e.OperationId,
            e.Name
        );

        RaiseEvent(operationAddedEvent);
        await ConfirmEvents();
    }
}
