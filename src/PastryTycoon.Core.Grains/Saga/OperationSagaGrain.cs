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

        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        operationSagaEventStream = streamProvider.GetStream<OperationSagaEvent>(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS, this.GetPrimaryKey());

        // Subscribe to our own events
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
        this.logger.LogInformation("OperationSagaGrain received event: {EventType}", item.GetType().Name);

        switch (item)
        {
            case ActivityAddedOnOperationEvent e:
                this.logger.LogInformation("Handling ActivityAddedOnOperationEvent for Operation ID: {OperationId}", e.OperationId);
                RaiseEvent(e);
                break;
            default:
                // Log or handle unrecognized event types
                break;
        }

        await ConfirmEvents();

    }

    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }

    public async Task SaveOperation(SaveOperationCommand command)
    {
        this.logger.LogInformation("Saving operation with ID: {OperationId}", command.OperationId);

        if (!State.IsActive)
        {
            var operationPendingEvent = new OperationPendingEvent(
                command.OperationId,
                command.Name
            );

            RaiseEvent(operationPendingEvent);
            await ConfirmEvents();

            if (operationSagaEventStream != null)
            {
                await operationSagaEventStream.OnNextAsync(operationPendingEvent);

                foreach (var activity in command.Activities)
                {
                    var activityId = Guid.NewGuid(); // Generate a new ID for the activity

                    var addActivityEvent = new AddActivityOnOperationEvent(
                        command.OperationId,
                        activityId,
                        activity
                    );

                    RaiseEvent(addActivityEvent);
                    await ConfirmEvents();
                    await operationSagaEventStream.OnNextAsync(addActivityEvent);
                }
            }

            operationSagaReminder = await this.RegisterOrUpdateReminder(
                "AddOperationCompletionReminder",
                TimeSpan.FromMinutes(1),
                TimeSpan.FromMinutes(1)
            );
        }
    }

    public async Task ReceiveReminder(string reminderName, TickStatus status)
    {
        this.logger.LogInformation("Received reminder: {ReminderName}", reminderName);

        if (reminderName == "AddOperationCompletionReminder")
        {
            if (State.IsActive && !State.IsCompleted && State.PendingActivityIds.Count == 0)
            {
                this.logger.LogInformation("Completing operation with ID: {OperationId}", State.PendingOperationId);

                // Raise an event to indicate the operation is completed
                RaiseEvent(new OperationCompletedEvent(State.PendingOperationId));
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
                this.logger.LogInformation("Unregistering reminder: {ReminderName}", reminderName);
                await this.UnregisterReminder(operationSagaReminder);
            }
        }
    }
}
