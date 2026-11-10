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
    if (string.IsNullOrWhiteSpace(recipe.Title))
    {
        throw new ArgumentException("O título da receita é obrigatório.");
    }

    _context.Recipes.Add(recipe);
    _context.SaveChanges();
}
}