using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public class OperationSagaState
{
    [Id(0)] public Guid PendingOperationId { get; set; }
    [Id(1)] public IList<Guid> PendingActivityIds { get; set; } = new List<Guid>();
    [Id(2)] public IList<Guid> CompletedActivityIds { get; set; } = new List<Guid>();
    [Id(3)] public bool IsActive { get; set; } = false;
    [Id(4)] public bool IsCompleted { get; set; } = false;

    public void Apply(OperationPendingEvent @event)
    {
        PendingOperationId = @event.OperationId;
        IsActive = true;
    }

    public void Apply(AddActivityOnOperationEvent @event)
    {
        if (PendingOperationId != @event.OperationId)
            throw new InvalidOperationException("Cannot add activity to a different operation.");

        PendingActivityIds.Add(@event.ActivityId);
    }

    public void Apply(ActivityAddedOnOperationEvent @event)
    {
        if (PendingOperationId != @event.OperationId)
            throw new InvalidOperationException("Cannot mark activity as added for a different operation.");

        if (!PendingActivityIds.Contains(@event.ActivityId))
            throw new InvalidOperationException("Activity not found in pending activities.");

        PendingActivityIds.Remove(@event.ActivityId);
        CompletedActivityIds.Add(@event.ActivityId);
    }

    public void Apply(OperationCompletedEvent @event)
    {
        if (PendingOperationId != @event.OperationId)
            throw new InvalidOperationException("Cannot complete a different operation.");

        IsActive = false;
        IsCompleted = true;
    }
}
