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

    public List<Ingredient> GetApprovedIngredients()
    {
        return _context.Ingredients
            .Where(ingredient => ingredient.Status == IngredientStatus.Approved)
            .OrderBy(ingredient => ingredient.Name)
            .ToList();
    }

    public void AddIngredient(Ingredient ingredient)
{
    if (string.IsNullOrWhiteSpace(ingredient.Name))
    {
        throw new ArgumentException(
            "O nome do ingrediente é obrigatório."
        );
    }

    bool alreadyExists = _context.Ingredients
        .Any(i => i.Name == ingredient.Name);

    if (alreadyExists)
    {
        throw new InvalidOperationException(
            "Este ingrediente já existe."
        );
    }

    ingredient.Name = ingredient.Name.Trim();
    ingredient.Status = IngredientStatus.Pending;

    _context.Ingredients.Add(ingredient);
    _context.SaveChanges();
}

}