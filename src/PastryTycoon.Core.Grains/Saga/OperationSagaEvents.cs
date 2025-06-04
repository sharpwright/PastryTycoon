using System;

namespace PastryTycoon.Core.Grains.Saga;

/// <summary>
/// Base class for operation saga events.
/// This class serves as a base for all events related to the operation saga.
/// </summary>
/// <param name="OperationSagaId"></param>
[GenerateSerializer]
public record OperationSagaEvent(Guid OperationSagaId);

/// <summary>
/// Event that is triggered when an operation is pending.
/// </summary>
/// <param name="OperationSagaId"></param>
/// <param name="OperationId"></param>
/// <param name="Name"></param>
[GenerateSerializer]
public record OperationPendingEvent(Guid OperationSagaId, Guid OperationId, string Name) : OperationSagaEvent(OperationSagaId);

/// <summary>
/// Event that is triggered when the saga is adding an activity to an operation.
/// </summary>
/// <param name="OperationSagaId"></param>
/// <param name="OperationId"></param>
/// <param name="ActivityId"></param>
/// <param name="Name"></param>
[GenerateSerializer]
public record AddActivityOnOperationEvent(Guid OperationSagaId, Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationSagaId);

/// <summary>
/// This event is used to notify the operation saga that an activity has been added.
/// </summary>
/// <param name="OperationSagaId"></param>
/// <param name="OperationId"></param>
/// <param name="ActivityId"></param>
/// <param name="Name"></param>
[GenerateSerializer]
public record ActivityAddedOnOperationEvent(Guid OperationSagaId, Guid OperationId, Guid ActivityId, string Name) : OperationSagaEvent(OperationSagaId);

/// <summary>
/// Event that is triggered when an operation saga is completed.
/// </summary>
/// <param name="OperationSagaId"></param>
[GenerateSerializer]
public record OperationSagaCompletedEvent(Guid OperationSagaId) : OperationSagaEvent(OperationSagaId);