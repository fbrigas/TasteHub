
using Microsoft.EntityFrameworkCore;
using TasteHub.Data;
using TasteHub.Entities;

namespace TasteHub.Services;

public class FavoriteService
{
    private readonly TasteHubDbContext _context;

    public FavoriteService(TasteHubDbContext context)
    {
        _context = context;
    }

    // Consultar os favoritos de um utilizador
    public List<Recipe> GetUserFavorites(int userId)
    {
        return _context.Favorites
            .Where(f => f.UserId == userId &&
                f.Recipe.Status == RecipeStatus.Approved)
            .Include(f => f.Recipe)
                .ThenInclude(r => r.Ingredients)
                    .ThenInclude(ri => ri.Ingredient)
            .Select(f => f.Recipe)
            .ToList();
    }

    // Verificar se uma receita é favorita
    public bool IsFavorite(int userId, int recipeId)
    {
        return _context.Favorites.Any(f =>
            f.UserId == userId &&
            f.RecipeId == recipeId);
    }

    // Adicionar uma receita aos favoritos
    public void AddFavorite(int userId, int recipeId)
    {
        Recipe? recipe = _context.Recipes
            .FirstOrDefault(r => r.Id == recipeId);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        if (recipe.Status != RecipeStatus.Approved)
        {
            throw new InvalidOperationException(
                "Só podes adicionar receitas aprovadas aos favoritos.");
        }

        if (IsFavorite(userId, recipeId))
        {
            throw new InvalidOperationException(
                "Esta receita já está nos teus favoritos.");
        }

        Favorite favorite = new Favorite
        {
            UserId = userId,
            RecipeId = recipeId
        };

        _context.Favorites.Add(favorite);
        _context.SaveChanges();
    }

    // Remover uma receita dos favoritos
    public void RemoveFavorite(int userId, int recipeId)
    {
        Favorite? favorite = _context.Favorites
            .FirstOrDefault(f =>
                f.UserId == userId &&
                f.RecipeId == recipeId);

        if (favorite == null)
        {
            throw new KeyNotFoundException(
                "Esta receita não está nos teus favoritos.");
        }

        _context.Favorites.Remove(favorite);
        _context.SaveChanges();
    }
}
