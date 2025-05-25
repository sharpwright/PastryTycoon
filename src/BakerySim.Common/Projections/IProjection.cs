using System;

namespace BakerySim.Common.Projections;

public interface IProjection
{
    Task EnsureActivated();
}
