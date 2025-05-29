using Orleans;

namespace PastryTycoon.Core.Abstractions.Player;

[GenerateSerializer]
public record DiscoverRecipeCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
);