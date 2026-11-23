using TasteHub.Data;
using TasteHub.Entities;

namespace TasteHub.Services;

public class IngredientService
{
    private readonly TasteHubDbContext _context;

    public IngredientService(TasteHubDbContext context)
    {
        _context = context;
    }

    // Obter todos os ingredientes aprovados
    public List<Ingredient> GetApprovedIngredients()
    {
        return _context.Ingredients
            .Where(ingredient => ingredient.Status == IngredientStatus.Approved)
            .OrderBy(ingredient => ingredient.Name)
            .ToList();
    }

    // Obter todos os ingredientes pendentes
    public List<Ingredient> GetPendingIngredients()
    {
        return _context.Ingredients
            .Where(ingredient => ingredient.Status == IngredientStatus.Pending)
            .OrderBy(ingredient => ingredient.Name)
            .ToList();
    }

    // Adicionar um novo ingrediente
    public void AddIngredient(Ingredient ingredient)
    {
        if (string.IsNullOrWhiteSpace(ingredient.Name))
        {
            throw new ArgumentException(
                "O nome do ingrediente é obrigatório."
            );
        }

        // Remover espaços antes de verificar duplicados
        ingredient.Name = ingredient.Name.Trim();

        bool alreadyExists = _context.Ingredients
            .Any(i => i.Name == ingredient.Name);

        if (alreadyExists)
        {
            throw new InvalidOperationException(
                "Este ingrediente já existe."
            );
        }

        // Todos os novos ingredientes ficam pendentes
        ingredient.Status = IngredientStatus.Pending;

        _context.Ingredients.Add(ingredient);
        _context.SaveChanges();
    }

    // Aprovar um ingrediente pendente
    public void ApproveIngredient(int id)
    {
        Ingredient? ingredient = _context.Ingredients
            .FirstOrDefault(i => i.Id == id);

        if (ingredient == null)
        {
            throw new KeyNotFoundException(
                "Ingrediente não encontrado."
            );
        }

        if (ingredient.Status != IngredientStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas ingredientes pendentes podem ser aprovados."
            );
        }

        ingredient.Status = IngredientStatus.Approved;

        _context.SaveChanges();
    }
    public void RejectIngredient(int id)
    {
        Ingredient? ingredient = _context.Ingredients
            .FirstOrDefault(i => i.Id == id);

        if (ingredient == null)
        {
            throw new KeyNotFoundException(
                "Ingrediente não encontrado."
            );
        }

        if (ingredient.Status != IngredientStatus.Pending)
        {
            throw new InvalidOperationException(
                "Apenas ingredientes pendentes podem ser rejeitados."
            );
        }

        ingredient.Status = IngredientStatus.Rejected;

        _context.SaveChanges();
    }

}