namespace TasteHub.Entities;

public class RecipeIngredient
{
    public int Id { get; set; }
    public int RecipeId { get; set; } // Foreign key to the Recipe entity
    public int IngredientId { get; set; } // Foreign key to the Ingredient entity
    public decimal Quantity { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public Ingredient Ingredient { get; set; } = null!;
    public string Unit { get; set; } = string.Empty; // Unit of measurement (e.g., "grams", "cups")


}