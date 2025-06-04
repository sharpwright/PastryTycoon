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

/// <summary>
/// Grain that represents an activity in the operation saga.
/// It listens for events related to operations and handles them accordingly.
/// </summary>
[LogConsistencyProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_PROVIDER)]
[StorageProvider(ProviderName = OrleansConstants.EVENT_SOURCING_LOG_STORAGE_ACTIVITY_EVENTS)]
[ImplicitStreamSubscription(OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS)]
public class ActivityGrain : JournaledGrain<ActivityState, ActivityEvent>, IActivityGrain,
    IAsyncObserver<OperationSagaEvent>,
    IStreamSubscriptionObserver
{
    private readonly ILogger<ActivityGrain> logger;

    /// <summary>
    /// Constructor for the ActivityGrain.
    /// Initializes the grain with a logger for logging purposes.
    /// </summary>
    /// <param name="logger"></param>
    public ActivityGrain(ILogger<ActivityGrain> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Adds an activity to the grain's state.
    /// This method raises an event that indicates an activity has been added.
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public async Task AddActivity(AddActivityCommand command)
    {
        this.logger.LogInformation("ActivityGrain adding activity with ID: {ActivityId}", command.ActivityId);
        this.logger.LogInformation("ActivityGrain primary key: {PrimaryKey}", this.GetPrimaryKey().ToString());

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

    /// <summary>
    /// Handles errors that occur during event processing.
    /// This method is called when an error occurs while processing events.
    /// </summary>
    /// <param name="ex"></param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public Task OnErrorAsync(Exception ex)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Handles the next event in the stream.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task OnNextAsync(OperationSagaEvent item, StreamSequenceToken? token = null)
    {
        this.logger.LogInformation("Activity received event: {EventType}", item.GetType().Name);

        switch (item)
        {
            case AddActivityOnOperationEvent e:
                await HandleAddActivityOnOperation(e);
                break;
            default:
                // this.logger.LogWarning("Unhandled event type: {EventType}", item.GetType().Name);
                return;
        }


    }
    
    /// <summary>
    /// Handles the AddActivityOnOperationEvent.
    /// This method is responsible for adding an activity to the operation and publishing an event
    /// </summary>
    /// <param name="e"></param>
    /// <returns></returns>
    private async Task HandleAddActivityOnOperation(AddActivityOnOperationEvent e)
    {
        // Call AddActivity with the command to ensure all necessary validations and state changes are applied.
        var command = new AddActivityCommand(
            e.ActivityId,
            e.Name,
            e.OperationId
        );
        await AddActivity(command);

        this.logger.LogInformation("ActivityGrain handled AddActivityOnOperationEvent for ActivityId: {ActivityId}", e.ActivityId);

        var activityAddedEvent = new ActivityAddedOnOperationEvent(
            e.OperationSagaId,
            e.OperationId,
            e.ActivityId,
            e.Name
        );

        var streamProvider = this.GetStreamProvider(OrleansConstants.AZURE_QUEUE_STREAM_PROVIDER);
        var operationSagaEventStream = streamProvider.GetStream<OperationSagaEvent>(
            OrleansConstants.STREAM_NAMESPACE_OPERATION_SAGA_EVENTS,
            e.OperationSagaId); // Use the unique OperationSagaId here!                            

        this.logger.LogInformation("Publishing ActivityAddedOnOperationEvent to stream for OperationId: {OperationId}", e.OperationId);
        this.logger.LogInformation("Activity grain primary key: {PrimaryKey}", this.GetPrimaryKey().ToString());

        await operationSagaEventStream.OnNextAsync(activityAddedEvent);
    }

    /// <summary>
    /// This method is called when the grain is subscribed to a stream.
    /// It resumes the stream subscription for OperationSagaEvent events.
    /// </summary>
    /// <param name="handleFactory"></param>
    /// <returns></returns>
    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<OperationSagaEvent>();
        await handle.ResumeAsync(this);
    }
}
