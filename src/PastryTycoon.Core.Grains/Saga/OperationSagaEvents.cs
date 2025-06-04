using System;

namespace PastryTycoon.Core.Grains.Saga;

[GenerateSerializer]
public record OperationSagaEvent(Guid OperationSagaId);
[GenerateSerializer]
public record OperationPendingEvent(Guid OperationSagaId, Guid OperationId, string Name) : OperationSagaEvent(OperationSagaId);
[GenerateSerializer]
public record AddActivityOnOperationEvent(Guid OperationSagaId, Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationSagaId);
[GenerateSerializer]
public record ActivityAddedOnOperationEvent(Guid OperationSagaId, Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationSagaId);
[GenerateSerializer]
public record OperationSagaCompletedEvent(Guid OperationSagaId) : OperationSagaEvent(OperationSagaId);