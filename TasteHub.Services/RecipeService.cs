
using Microsoft.EntityFrameworkCore;
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

    private IQueryable<Recipe> RecipesWithIngredients()
    {
        return _context.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient);
    }

    public List<Recipe> GetAllRecipes()
    {
        return RecipesWithIngredients().ToList();
    }

    public List<Recipe> GetApprovedRecipes()
    {
        return RecipesWithIngredients()
            .Where(r => r.Status == RecipeStatus.Approved)
            .ToList();
    }

    public List<Recipe> GetRecipesByUserId(int userId)
    {
        return RecipesWithIngredients()
            .Where(r => r.UserId == userId)
            .ToList();
    }

    public Recipe? GetRecipeById(int id)
    {
        return RecipesWithIngredients()
            .FirstOrDefault(r => r.Id == id);
    }

    public void AddRecipe(Recipe recipe)
    {
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException(
                "O título da receita é obrigatório.");
        }

        if (recipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException(
                "A duração deve ser superior a zero.");
        }

        if (recipe.Ingredients == null ||
            recipe.Ingredients.Count == 0)
        {
            throw new ArgumentException(
                "Adiciona pelo menos um ingrediente.");
        }

        var existingIds = recipe.Ingredients
            .Where(ri => ri.IngredientId > 0)
            .Select(ri => ri.IngredientId)
            .ToList();

        if (existingIds.Count != existingIds.Distinct().Count())
        {
            throw new ArgumentException(
                "A receita não pode conter ingredientes repetidos.");
        }

        var newNames = new HashSet<string>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var item in recipe.Ingredients)
        {
            if (item.Quantity <= 0 ||
                string.IsNullOrWhiteSpace(item.Unit))
            {
                throw new ArgumentException(
                    "Todos os ingredientes precisam de quantidade e unidade.");
            }

            item.Unit = item.Unit.Trim();

            if (item.IngredientId > 0)
            {
                var existing = _context.Ingredients
                    .FirstOrDefault(i =>
                        i.Id == item.IngredientId);

                if (existing == null)
                {
                    throw new ArgumentException(
                        $"O ingrediente com ID {item.IngredientId} não existe.");
                }

                if (existing.Status != IngredientStatus.Approved)
                {
                    throw new ArgumentException(
                        $"O ingrediente '{existing.Name}' não está aprovado.");
                }
            }
            else
            {
                if (item.Ingredient == null ||
                    string.IsNullOrWhiteSpace(item.Ingredient.Name))
                {
                    throw new ArgumentException(
                        "O nome do novo ingrediente é obrigatório.");
                }

                string name = item.Ingredient.Name.Trim();

                if (name.Length < 2 || name.Length > 100)
                {
                    throw new ArgumentException(
                        "O nome do ingrediente deve ter entre 2 e 100 caracteres.");
                }

                if (!newNames.Add(name))
                {
                    throw new ArgumentException(
                        $"O ingrediente '{name}' está repetido.");
                }

                bool exists = _context.Ingredients
                    .Any(i => i.Name.ToLower() == name.ToLower());

                if (exists)
                {
                    throw new ArgumentException(
                        $"O ingrediente '{name}' já existe. Não podes sugeri-lo novamente.");
                }

                item.Ingredient.Name = name;
                item.Ingredient.Status = IngredientStatus.Pending;
            }
        }

        using var transaction = _context.Database.BeginTransaction();

        try
        {
            recipe.Status = RecipeStatus.Draft;

            _context.Recipes.Add(recipe);
            _context.SaveChanges();

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public void UpdateRecipe(int id, Recipe updatedRecipe)
    {
        Recipe? existingRecipe = _context.Recipes.Find(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        if (existingRecipe.Status != RecipeStatus.Draft &&
            existingRecipe.Status != RecipeStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Apenas receitas em rascunho ou rejeitadas podem ser editadas.");
        }

        if (string.IsNullOrWhiteSpace(updatedRecipe.Title))
        {
            throw new ArgumentException(
                "O título da receita é obrigatório.");
        }

        if (updatedRecipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException(
                "A duração deve ser superior a zero.");
        }

        existingRecipe.Title = updatedRecipe.Title;
        existingRecipe.Description = updatedRecipe.Description;
        existingRecipe.Instructions = updatedRecipe.Instructions;
        existingRecipe.DurationInMinutes =
            updatedRecipe.DurationInMinutes;
        existingRecipe.Difficulty = updatedRecipe.Difficulty;
        existingRecipe.Category = updatedRecipe.Category;

        _context.SaveChanges();
    }

    public void DeleteRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();
    }

    public void SubmitRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        if (recipe.Status != RecipeStatus.Draft &&
            recipe.Status != RecipeStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Esta receita não pode ser submetida neste estado.");
        }

        recipe.Status = RecipeStatus.Pending;
        _context.SaveChanges();
    }

    public void ApproveRecipe(int id)
    {
        Recipe? recipe = RecipesWithIngredients()
            .FirstOrDefault(r => r.Id == id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        if (recipe.Status != RecipeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas receitas pendentes podem ser aprovadas.");
        }

        var unapprovedIngredients = recipe.Ingredients
            .Where(ri =>
                ri.Ingredient.Status != IngredientStatus.Approved)
            .Select(ri => ri.Ingredient.Name)
            .ToList();

        if (unapprovedIngredients.Count > 0)
        {
            throw new InvalidOperationException(
                "Não é possível aprovar esta receita. " +
                "Existem ingredientes pendentes ou rejeitados: " +
                string.Join(", ", unapprovedIngredients));
        }

        recipe.Status = RecipeStatus.Approved;
        _context.SaveChanges();
    }

    public void RejectRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada.");
        }

        if (recipe.Status != RecipeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas receitas pendentes podem ser rejeitadas.");
        }

        recipe.Status = RecipeStatus.Rejected;
        _context.SaveChanges();
    }
}
