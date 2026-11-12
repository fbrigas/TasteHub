using TasteHub.Data;
using TasteHub.Entities;

namespace TasteHub.Services;

public class RecipeService
{
    private readonly TasteHubDbContext _context;

    public RecipeService(TasteHubDbContext context)
    {
        _context = context;
    }

    public List<Recipe> GetAllRecipes()
    {
        return _context.Recipes.ToList();
    }

    public Recipe? GetRecipeById(int id)
    {
        return _context.Recipes.Find(id);
    }

    public void AddRecipe(Recipe recipe)
    {
        // 1. Validar o título
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException("O título da receita é obrigatório.");
        }

        // 2. Validar a duração
        if (recipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException("A duração deve ser superior a zero.");
        }

        // 3. Adicionar a receita
        _context.Recipes.Add(recipe);

        // 4. Guardar na base de dados
        _context.SaveChanges();
    }
    public void UpdateRecipe(int id, Recipe updatedRecipe)
    {
        Recipe? existingRecipe = _context.Recipes.Find(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException("Receita não encontrada.");
        }

        if (string.IsNullOrWhiteSpace(updatedRecipe.Title))
        {
            throw new ArgumentException("O título da receita é obrigatório.");
        }
        if (updatedRecipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException("A duração deve ser superior a zero.");
        }

        existingRecipe.Title = updatedRecipe.Title;
        existingRecipe.DurationInMinutes = updatedRecipe.DurationInMinutes;
        existingRecipe.Description = updatedRecipe.Description;
        existingRecipe.Instructions = updatedRecipe.Instructions;
        existingRecipe.Difficulty = updatedRecipe.Difficulty;
        existingRecipe.Category = updatedRecipe.Category;

        _context.SaveChanges();
    }

    public void DeleteRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException("Receita não encontrada.");
        }

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();
    }

}