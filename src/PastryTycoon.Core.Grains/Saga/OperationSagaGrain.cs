using System;
using System.Runtime.Serialization;
using Microsoft.Extensions.Logging;
using Orleans.EventSourcing;
using Orleans.Providers;
using Orleans.Streams;
using PastryTycoon.Core.Abstractions.Constants;
using PastryTycoon.Core.Abstractions.Saga;

namespace PastryTycoon.Core.Grains.Saga;

[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_OPERATION_SAGA_EVENTS)]
public class OperationSagaGrain : JournaledGrain<OperationSagaState, OperationSagaEvent>, IOperationSagaGrain, IRemindable, IAsyncObserver<OperationSagaEvent>
{
    private IGrainReminder? operationSagaReminder;
    private IStreamProvider streamProvider = null!; // Initialized in OnActivateAsync
    private IAsyncStream<OperationSagaEvent>? operationSagaEventStream;
    
    private StreamSubscriptionHandle<OperationSagaEvent>? subscriptionHandle;
    private readonly ILogger<OperationSagaGrain> logger;

    public OperationSagaGrain(ILogger<OperationSagaGrain> logger)
    {
        this.logger = logger;
    }

    public override async Task OnActivateAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Activating OperationSagaGrain with ID: {GrainId}", this.GetPrimaryKey());

        // Subscribe to our own events.
        streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        operationSagaEventStream = streamProvider.GetStream<OperationSagaEvent>(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS, this.GetPrimaryKey());
        subscriptionHandle = await operationSagaEventStream.SubscribeAsync(this);

        await base.OnActivateAsync(cancellationToken);
    }

    public override async Task OnDeactivateAsync(DeactivationReason reason, CancellationToken cancellationToken)
    {
        this.logger.LogInformation("Deactivating OperationSagaGrain with ID: {GrainId}, Reason: {Reason}", this.GetPrimaryKey(), reason);

        if (subscriptionHandle != null)
        {
            await subscriptionHandle.UnsubscribeAsync();
        }
        await base.OnDeactivateAsync(reason, cancellationToken);
    }

    public async Task OnNextAsync(OperationSagaEvent item, StreamSequenceToken? token = null)
    {
        this.logger.LogInformation(
            "OperationSagaGrain (primary key: {PrimaryKey}) received event: {EventType} for OperationSagaId: {OperationSagaId}",
            this.GetPrimaryKey(),
            item.GetType().Name,
            item.OperationSagaId);

        switch (item)
        {
            case ActivityAddedOnOperationEvent e:
                this.logger.LogInformation(
                    "Handling ActivityAddedOnOperationEvent for Operation ID: {OperationId}",
                    e.OperationId);

                RaiseEvent(e);
                break;
            default:
                // Log or handle unrecognized event types
                this.logger.LogWarning(
                    "OperationSagaGrain unhandled event type: {EventType} for OperationSagaId: {OperationSagaId}", 
                    item.GetType().Name,
                    item.OperationSagaId);
                break;
        }

        await ConfirmEvents();

    }

    public Task OnErrorAsync(Exception ex)
    {
        this.logger.LogError(ex,
            "Error in OperationSagaGrain stream subscription for primary key: {PrimaryKey}",
            this.GetPrimaryKey());

        throw new NotImplementedException();
    }

    public async Task SaveOperation(SaveOperationCommand command)
    {
        this.logger.LogInformation(
            "OperationSagaGrain received SaveOperationCommand for Operation ID: {OperationId}",
            command.OperationId);

        if (!State.IsActive)
        {
            var operationPendingEvent = new OperationPendingEvent(
                this.GetPrimaryKey(),
                command.OperationId,
                command.Name
            );

            RaiseEvent(operationPendingEvent);
            await ConfirmEvents();

            var operationEventStream = streamProvider.GetStream<OperationSagaEvent>(
                OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS,
                command.OperationId); // Use the unique OperationId here!        

            this.logger.LogInformation(
                "OperationSagaGrain sending OperationPendingEvent for Operation ID: {OperationId} to stream",
                command.OperationId);

            await operationEventStream.OnNextAsync(operationPendingEvent);
                
            foreach (var activity in command.Activities)
            {
                var activityId = Guid.NewGuid(); // Generate a new ID for the activity

                var addActivityEvent = new AddActivityOnOperationEvent(
                    this.GetPrimaryKey(),
                    command.OperationId,
                    activityId,
                    activity
                );

                RaiseEvent(addActivityEvent);
                await ConfirmEvents();

                var activityStream = streamProvider.GetStream<OperationSagaEvent>(
                    OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS,
                    activityId // Use the unique ActivityId here!
                );

                this.logger.LogInformation(
                    "OperationSagaGrain sending AddActivityOnOperationEvent for Activity ID: {ActivityId} to stream",
                    activityId);

                await activityStream.OnNextAsync(addActivityEvent);
            }

            // Register a reminder to check for operation completion
            this.logger.LogInformation("OperationSagaGrain registering reminder for operation completion check.");
            operationSagaReminder = await this.RegisterOrUpdateReminder(
                "AddOperationCompletionReminder",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)
            );
        }
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        this.logger.LogInformation("OperationSagaGrain received reminder: {ReminderName}", reminderName);

        if (reminderName == "AddOperationCompletionReminder")
        {
            if (State.IsActive && !State.IsCompleted && State.PendingActivityIds.Count == 0)
            {
                this.logger.LogInformation("OperationSagaGrain completing operation with ID: {OperationId}", State.PendingOperationId);

                // Raise an event to indicate the operation is completed
                RaiseEvent(new OperationSagaCompletedEvent(this.GetPrimaryKey()));
                await ConfirmEvents();

                if (operationSagaReminder == null)
                {
                    operationSagaReminder = await this.GetReminder("AddOperationCompletionReminder");

                    if (operationSagaReminder == null)
                    {
                        throw new InvalidOperationException("Reminder not found.");
                    }
                }

                // Unregister the reminder since the operation is completed
                this.logger.LogInformation("OperationSagaGrain unregistering reminder: {ReminderName}", reminderName);
                await this.UnregisterReminder(operationSagaReminder);
            }
        }
    }
}
