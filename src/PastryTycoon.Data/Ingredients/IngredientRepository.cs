using System;
using System.ComponentModel;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;

namespace PastryTycoon.Data.Ingredients;

public record Datasource(
    [property: JsonPropertyName("ingredients")] IReadOnlyList<Ingredient> Ingredients
);

public record Ingredient(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("description")] string Description,
    [property: JsonPropertyName("state")] string State,
    [property: JsonPropertyName("categories")] IReadOnlyList<string> Categories,
    [property: JsonPropertyName("unit")] string Unit    
);


/// <summary>
/// Repository for managing ingredients.
/// For now the class loads ingredient data from an embedded JSON resource.
/// In the future it may have to use a database or other data source.
/// </summary>
public class IngredientRepository : IIngredientRepository
{
    private const string ResourceName = "PastryTycoon.Data.Ingredients.Datasource.json";
    private readonly ILogger<IngredientRepository> logger;

    private IReadOnlyList<Ingredient> ingredients { get; set; } = new List<Ingredient>();

    /// <summary>
    /// Initializes a new instance of the <see cref="IngredientRepository"/> class.
    /// Loads ingredient data from an embedded JSON resource.
    /// </summary>
    public IngredientRepository(ILogger<IngredientRepository> logger)
    {
        this.logger = logger;

        var assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(ResourceName);

        if (stream == null)
        {
            var ex = new FileNotFoundException($"Embedded resource '{ResourceName}' not found.");
            logger.LogError(ex, "Failed to load ingredient data.");
            throw ex;
        }

        var datasource = JsonSerializer.Deserialize<Datasource>(stream);
        this.ingredients = datasource?.Ingredients ?? new List<Ingredient>();
    }

    /// <summary>
    /// Asynchronously retrieves all ingredients from the repository.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a read-only list of <see cref="Ingredients"/>.</returns>
    public Task<IReadOnlyList<Ingredient>> GetAllIngredientsAsync()
    {
        return Task.FromResult(ingredients);
    }

    /// <summary>
    /// Asynchronously retrieves an ingredient by its ID.
    /// </summary>
    /// <param name="ingredientId"></param>
    /// <returns></returns>
    public Task<Ingredient?> GetIngredientByIdAsync(string ingredientId)
    {
        if (string.IsNullOrEmpty(ingredientId))
        {
            throw new ArgumentException("Ingredient ID cannot be null or empty.", nameof(ingredientId));
        }

        var ingredient = ingredients.FirstOrDefault(i => i.Id.Equals(ingredientId));
        return Task.FromResult(ingredient);
    }

    /// <summary>
    /// Asynchronously retrieves a list of ingredients by their IDs.
    /// </summary>
    /// <param name="ingredientIds"></param>
    /// <returns></returns>
    public Task<IList<Ingredient>> GetIngredientsByIdsAsync(params string[] ingredientIds)
    {
        if (ingredientIds == null || ingredientIds.Length == 0)
        {
            throw new ArgumentException("Ingredient IDs cannot be null or empty.", nameof(ingredientIds));
        }
        
        var ingredientList = ingredients
            .Where(i => ingredientIds.Contains(i.Id))
            .ToList();
        return Task.FromResult<IList<Ingredient>>(ingredientList);
    }
}
