using System;

namespace PastryTycoon.Core.Grains.Common;

/// <summary>
/// Represents the context for validating a grain command against its state.
/// </summary>
/// <typeparam name="TCommand">The type of the command being validated.</typeparam>
/// <typeparam name="TState">The type of the grain state being validated against.</typeparam>
/// <typeparam name="TGrainPrimaryKey">The type of the grain's primary key.</typeparam>
public class GrainValidationContext<TCommand, TState, TGrainPrimaryKey>
{
    /// <summary>
    /// The command being validated.
    /// </summary>
    public TCommand Command { get; }

    /// <summary>
    /// The current state of the grain being validated against the command.
    /// </summary>
    public TState GrainState { get; }

    /// <summary>
    /// The primary key of the grain being validated.
    /// </summary>
    public TGrainPrimaryKey GrainPrimaryKey { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="GrainValidationContext{TCommand, TState, TGrainPrimaryKey}"/> class.
    /// </summary>
    /// <param name="command"></param>
    /// <param name="grainState"></param>
    /// <param name="grainPrimaryKey"></param>
    public GrainValidationContext(TCommand command, TState grainState, TGrainPrimaryKey grainPrimaryKey)
    {
        Command = command;
        GrainState = grainState;
        GrainPrimaryKey = grainPrimaryKey;
    }

}
