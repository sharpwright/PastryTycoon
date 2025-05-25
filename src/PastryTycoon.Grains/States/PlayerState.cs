using System;
using PastryTycoon.Grains.Events;

namespace PastryTycoon.Grains.States;

public class PlayerState
{
    public Guid PlayerId { get; set; }
    public IDictionary<Guid, DateTime> KnownRecipeIds { get; set; } = new Dictionary<Guid, DateTime>();
    public IDictionary<string, DateTime> Achievements { get; set; } = new Dictionary<string, DateTime>();
    public decimal Balance { get; set; } = 0.0m;

    // Event sourcing: apply RecipeDiscoveredEvent
    public void Apply(RecipeDiscoveredEvent evt)
    {
        KnownRecipeIds[evt.RecipeId] = evt.DiscoveryTimeUtc;
    }

    public void Apply(AchievementUnlockedEvent evt)
    {
        Achievements[evt.Achievement] = evt.UnlockedAtUtc;
    }

}
