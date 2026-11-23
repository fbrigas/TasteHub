using System.ComponentModel.DataAnnotations;

namespace TasteHub.Api.DTOs;

public class RecipeIngredientDto
{
    [Range(1, int.MaxValue)]
    public int IngredientId { get; set; }

    [Range(typeof(decimal), "0.001", "999999")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;
}