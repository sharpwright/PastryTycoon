using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public class ActivityState
{
    [Id(0)]
    public Guid ActivityId { get; set; }

    [Id(1)]
    public Guid OperationId { get; set; }

    [Id(2)]
    public string Name { get; set; } = string.Empty;

    [Id(3)]
    public DateTime CreatedAt { get; set; }

    public ActivityState()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public void Apply(ActivityAddedEvent activityAddedEvent)
    {
        ActivityId = activityAddedEvent.ActivityId;
        Name = activityAddedEvent.Name;
        OperationId = activityAddedEvent.OperationId;
    }
}
