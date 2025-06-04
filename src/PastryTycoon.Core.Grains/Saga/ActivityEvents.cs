using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public record ActivityEvent(Guid ActivityId);

[GenerateSerializer]
public record ActivityAddedEvent(Guid ActivityId, string Name, Guid OperationId) : ActivityEvent(ActivityId);
