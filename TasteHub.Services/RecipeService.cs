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

    // Consultar todas as receitas (administrador)
    public List<Recipe> GetAllRecipes()
    {
        return _context.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .ToList();
    }

    // Consultar apenas receitas aprovadas
    public List<Recipe> GetApprovedRecipes()
    {
        return _context.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Where(recipe => recipe.Status == RecipeStatus.Approved)
            .ToList();
    }

    // Consultar receitas de um utilizador
    public List<Recipe> GetRecipesByUserId(int userId)
    {
        return _context.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .Where(recipe => recipe.UserId == userId)
            .ToList();
    }

    // Consultar uma receita pelo ID
    public Recipe? GetRecipeById(int id)
    {
        return _context.Recipes
            .Include(r => r.Ingredients)
            .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefault(r => r.Id == id);
    }

    // Criar uma receita
    public void AddRecipe(Recipe recipe)
    {
        if (string.IsNullOrWhiteSpace(recipe.Title))
        {
            throw new ArgumentException(
                "O título da receita é obrigatório."
            );
        }

        if (recipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException(
                "A duração deve ser superior a zero."
            );
        }

        // Verificar se existem ingredientes repetidos
        List<int> ingredientIds = recipe.Ingredients
            .Select(i => i.IngredientId)
            .ToList();

        if (ingredientIds.Count != ingredientIds.Distinct().Count())
        {
            throw new ArgumentException(
                "A receita não pode conter ingredientes repetidos."
            );
        }

        // Verificar se os ingredientes existem e estão aprovados
        foreach (RecipeIngredient recipeIngredient in recipe.Ingredients)
        {
            Ingredient? ingredient = _context.Ingredients
                .FirstOrDefault(i => i.Id == recipeIngredient.IngredientId);

            if (ingredient == null)
            {
                throw new ArgumentException(
                    $"O ingrediente com ID {recipeIngredient.IngredientId} não existe."
                );
            }

            if (ingredient.Status != IngredientStatus.Approved)
            {
                throw new ArgumentException(
                    $"O ingrediente '{ingredient.Name}' ainda não está aprovado."
                );
            }
        }

        _context.Recipes.Add(recipe);
        _context.SaveChanges();
    }

    // Atualizar uma receita
    public void UpdateRecipe(int id, Recipe updatedRecipe)
    {
        Recipe? existingRecipe = _context.Recipes.Find(id);

        if (existingRecipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada."
            );
        }

        // Apenas rascunhos e receitas rejeitadas podem ser editados
        if (existingRecipe.Status != RecipeStatus.Draft &&
            existingRecipe.Status != RecipeStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Apenas receitas em rascunho ou rejeitadas podem ser editadas."
            );
        }

        if (string.IsNullOrWhiteSpace(updatedRecipe.Title))
        {
            throw new ArgumentException(
                "O título da receita é obrigatório."
            );
        }

        if (updatedRecipe.DurationInMinutes <= 0)
        {
            throw new ArgumentException(
                "A duração deve ser superior a zero."
            );
        }

        existingRecipe.Title = updatedRecipe.Title;
        existingRecipe.DurationInMinutes = updatedRecipe.DurationInMinutes;
        existingRecipe.Description = updatedRecipe.Description;
        existingRecipe.Instructions = updatedRecipe.Instructions;
        existingRecipe.Difficulty = updatedRecipe.Difficulty;
        existingRecipe.Category = updatedRecipe.Category;

        _context.SaveChanges();
    }

    // Eliminar uma receita
    public void DeleteRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada."
            );
        }

        _context.Recipes.Remove(recipe);
        _context.SaveChanges();
    }

    // Submeter uma receita para aprovação
    public void SubmitRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada."
            );
        }

        if (recipe.Status != RecipeStatus.Draft &&
            recipe.Status != RecipeStatus.Rejected)
        {
            throw new InvalidOperationException(
                "Esta receita não pode ser submetida neste estado."
            );
        }

        recipe.Status = RecipeStatus.Pending;

        _context.SaveChanges();
    }

    // Aprovar uma receita
    public void ApproveRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada."
            );
        }

        if (recipe.Status != RecipeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas receitas pendentes podem ser aprovadas."
            );
        }

        recipe.Status = RecipeStatus.Approved;

        _context.SaveChanges();
    }

    // Rejeitar uma receita
    public void RejectRecipe(int id)
    {
        Recipe? recipe = _context.Recipes.Find(id);

        if (recipe == null)
        {
            throw new KeyNotFoundException(
                "Receita não encontrada."
            );
        }

        if (recipe.Status != RecipeStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas receitas pendentes podem ser rejeitadas."
            );
        }

        recipe.Status = RecipeStatus.Rejected;

        _context.SaveChanges();
    }
}
