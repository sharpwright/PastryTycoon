using System;
using System.Collections;
using Orleans.EventSourcing;
using PastryTycoon.Grains.Actors;

namespace PastryTycoon.Grains.UnitTests.TestClusterHelpers;

public static class JournaledGrainTestExtensions
{
    public static TState GetInternalState<TState>(this IGrainWithGuidKey grain)
        where TState : class, new()
    {var stateProp = grain.GetType().GetProperty("State", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
        

        if (stateProp == null)
        {
            throw new InvalidOperationException($"No 'State' property found on type {grain.GetType().FullName}.");
        }

        if (stateProp is not TState)    
        {
            throw new InvalidOperationException($"The 'State' property on type {grain.GetType().FullName} is not of type {typeof(TState).FullName}.");
        }

        var value = stateProp.GetValue(grain);

        if (value is null)
        {
            throw new InvalidOperationException($"The 'State' property on type {grain.GetType().FullName} is null.");            
        }

        return (TState)value; 
    }
}