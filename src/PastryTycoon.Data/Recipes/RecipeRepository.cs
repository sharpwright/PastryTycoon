using System;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace PastryTycoon.Data.Recipes;

public record Datasource(
    [property: JsonPropertyName("recipes")] List<Recipe> Recipes
);

public record Recipe(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("ingredients")] List<RecipeIngredient> Ingredients,
    [property: JsonPropertyName("optionalIngredients")] List<OptionalRecipeIngredient>? OptionalIngredients = null,
    [property: JsonPropertyName("producesIngredientId")] string? ProducesIngredientId = null
);

public record RecipeIngredient(
    [property: JsonPropertyName("ingredientId")] string? IngredientId,
    [property: JsonPropertyName("category")] string? Category,
    [property: JsonPropertyName("amount")] int Amount
);

public record OptionalRecipeIngredient(
    [property: JsonPropertyName("ingredientId")] string? IngredientId,
    [property: JsonPropertyName("category")] string? Category,
    [property: JsonPropertyName("amount")] int Amount,
    [property: JsonPropertyName("qualityBoost")] int QualityBoost
);

/// <summary>
/// Repository for managing recipes.
/// For now the class loads recipe data from an embedded JSON resource.
/// In the future it may have to use a database or other data source.
/// </summary>
public class RecipeRepository : IRecipeRepository
{
    private const string ResourceName = "PastryTycoon.Data.Recipes.Datasource.json";
    private readonly ILogger<RecipeRepository> logger;

    private IReadOnlyList<Recipe> recipes { get; set; } = new List<Recipe>();

    /// <summary>
    /// Initializes a new instance of the <see cref="RecipeRepository"/> class.
    /// Loads recipe data from an embedded JSON resource.
    /// </summary>
    public RecipeRepository(ILogger<RecipeRepository> logger)
    {
        this.logger = logger;

        // Constructor logic if needed
        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(ResourceName);

        if (stream == null)
        {
            var ex = new FileNotFoundException($"Embedded resource '{ResourceName}' not found.");
            logger.LogError(ex, "Failed to load recipe data.");
            throw ex;
        }

        var datasource = JsonSerializer.Deserialize<Datasource>(stream);
        this.recipes = datasource?.Recipes ?? new List<Recipe>();
    }

    /// <summary>
    /// Asynchronously retrieves all recipes from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="Recipe"/>.</returns>
    public Task<IReadOnlyList<Recipe>> GetAllRecipesAsync()
    {
        return Task.FromResult(recipes);
    }

    public Task<Recipe?> GetRecipeByIdAsync(string recipeId)
    {
        if (string.IsNullOrEmpty(recipeId))
        {
            throw new ArgumentException("Recipe ID cannot be null or empty.", nameof(recipeId));
        }

        var recipe = recipes.FirstOrDefault(r => r.Id.Equals(recipeId, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult(recipe);
    }

    public Task<Recipe?> GetRecipeByIngredientIdsAsync(IList<string> ingredientIds)
    {
        if (ingredientIds == null || ingredientIds.Count == 0)
        {
            throw new ArgumentException("Ingredient IDs cannot be null or empty.", nameof(ingredientIds));
        }

        var recipe = recipes.FirstOrDefault(r =>
            r.Ingredients.All(i => i.IngredientId != null && ingredientIds.Contains(i.IngredientId)) &&
            r.Ingredients.Count == ingredientIds.Count
        );
        return Task.FromResult(recipe);
    }
}
