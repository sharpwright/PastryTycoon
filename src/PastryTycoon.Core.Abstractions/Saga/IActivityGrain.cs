using System;
using Orleans;

namespace PastryTycoon.Core.Abstractions.Saga;

public interface IActivityGrain : IGrainWithGuidKey
{
    Task AddActivity(AddActivityCommand command);
}
