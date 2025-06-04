using System;

namespace PastryTycoon.Core.Grains.Saga;

/// <summary>
/// Base class for activity events.
/// </summary>
/// <param name="ActivityId"></param>
[GenerateSerializer]
public record ActivityEvent(Guid ActivityId);

/// <summary>
/// Event that is triggered when an activity is added.
/// </summary>
/// <param name="ActivityId"></param>
/// <param name="Name"></param>
/// <param name="OperationId"></param>
[GenerateSerializer]
public record ActivityAddedEvent(Guid ActivityId, string Name, Guid OperationId) : ActivityEvent(ActivityId);
