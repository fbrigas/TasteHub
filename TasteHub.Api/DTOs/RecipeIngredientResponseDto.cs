namespace TasteHub.Api.DTOs;

public class RecipeIngredientResponseDto
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public string Unit { get; set; } = string.Empty;
}