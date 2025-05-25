using System;

namespace PastryTycoon.Data.Ingredients;

public interface IIngredientRepository
{
    Task<IReadOnlyList<Ingredient>> GetAllIngredientsAsync();
}