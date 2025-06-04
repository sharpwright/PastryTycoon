using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Saga;

public interface IOperationSagaGrain : IGrainWithGuidKey
{
    Task SaveOperation(SaveOperationCommand command);
}
