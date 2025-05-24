namespace BakerySim.Grains.Commands;

[GenerateSerializer]
public record DiscoverRecipeCommand(
    [property: Id(0)] Guid PlayerId,
    [property: Id(1)] Guid RecipeId,
    [property: Id(2)] DateTime DiscoveryTimeUtc
);