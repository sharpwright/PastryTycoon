using System;

namespace PastryTycoon.Grains.Validation;

public class GrainValidationContext<TCommand, TState, TGrainPrimaryKey>
{
    public TCommand Command { get; }
    public TState GrainState { get; }
    public TGrainPrimaryKey GrainPrimaryKey { get; }

    public GrainValidationContext(TCommand command, TState grainState, TGrainPrimaryKey grainPrimaryKey)
    {
        Command = command;
        GrainState = grainState;
        GrainPrimaryKey = grainPrimaryKey;
    }

}
