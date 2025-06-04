using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public record OperationSagaEvent(Guid OperationId);
[GenerateSerializer]
public record OperationPendingEvent(Guid OperationId, string Name) : OperationSagaEvent(OperationId);
[GenerateSerializer]
public record AddActivityOnOperationEvent(Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationId);
[GenerateSerializer]
public record ActivityAddedOnOperationEvent(Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationId);
[GenerateSerializer]
public record OperationCompletedEvent(Guid OperationId) : OperationSagaEvent(OperationId);