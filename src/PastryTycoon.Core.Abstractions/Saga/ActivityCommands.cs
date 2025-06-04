using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Saga;

[GenerateSerializer]
public record AddActivityCommand(Guid ActivityId, string Name, Guid OperationId);