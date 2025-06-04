using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public record OperationEvent(Guid OperationId);

[GenerateSerializer]
public record OperationAddedEvent(Guid OperationId, string Name) : OperationEvent(OperationId);