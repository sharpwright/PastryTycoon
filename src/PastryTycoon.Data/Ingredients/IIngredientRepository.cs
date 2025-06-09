using System;

namespace PastryTycoon.Data.Ingredients;

public interface IIngredientRepository
{
    Task<IReadOnlyList<Ingredient>> GetAllIngredientsAsync();
    Task<Ingredient?> GetIngredientByIdAsync(string ingredientId);
    Task<IList<Ingredient>> GetIngredientsByIdsAsync(params string[] ingredientIds);
}