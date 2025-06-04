using System;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using Orleans.Streams.Core;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Saga;

namespace PastryTycoon.Core.Grains.Saga;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_ACTIVITY_EVENTS)]
[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS)]
public class ActivityGrain : JournaledGrain<ActivityState, ActivityEvent>, IActivityGrain,
    IAsyncObserver<OperationSagaEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<ActivityGrain> logger;

    public ActivityGrain(ILogger<ActivityGrain> logger)
    {
        this.logger = logger;
    }

    public async Task AddActivity(AddActivityCommand command)
    {
        this.logger.LogInformation("Adding activity with ID: {ActivityId}", command.ActivityId);

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var evt = new ActivityAddedEvent(
            command.ActivityId,
            command.Name,
            command.OperationId
        );

        RaiseEvent(evt);
        await ConfirmEvents();
    }

    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }

    public async Task OnNextAsync(OperationSagaEvent item, StreamSequenceToken? token = null)
    {
        this.logger.LogInformation("Activity received event: {EventType}", item.GetType().Name);

        switch (item)
        {
            case AddActivityOnOperationEvent e:
                await HandleAddActivityOnOperation(e);
                break;
            default:
                this.logger.LogWarning("Unhandled event type: {EventType}", item.GetType().Name);
                return;
        }


    }

    public async Task HandleAddActivityOnOperation(AddActivityOnOperationEvent e)
    {
        var evt = new ActivityAddedEvent(
            e.ActivityId,
            e.Name,
            e.OperationId
        );

        RaiseEvent(evt);
        await ConfirmEvents();

        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var operationSagaEventStream = streamProvider.GetStream<OperationSagaEvent>(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS, this.GetPrimaryKey());

        if (operationSagaEventStream != null)
        {
            this.logger.LogInformation("Publishing ActivityAddedOnOperationEvent to stream for OperationId: {OperationId}", e.OperationId);
            
            var activityAddedEvent = new ActivityAddedOnOperationEvent(
                e.OperationId,
                e.ActivityId,
                e.Name
            );
            await operationSagaEventStream.OnNextAsync(activityAddedEvent);
        }

    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<OperationSagaEvent>();
        await handle.ResumeAsync(this);
    }
}
