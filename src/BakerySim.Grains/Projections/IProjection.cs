using System;

namespace BakerySim.Grains.Projections;

public interface IProjection
{
    Task EnsureActivated();
}
