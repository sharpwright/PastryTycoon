using System;

namespace PastryTycoon.Data.Recipes;

public interface IRecipeRepository
{
    Task<IReadOnlyList<Recipe>> GetAllRecipesAsync();
    Task<Recipe?> GetRecipeByIdAsync(string recipeId);
}
