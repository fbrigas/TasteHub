
using System.ComponentModel.DataAnnotations;

namespace TasteHub.Api.DTOs;

public class NewRecipeIngredientDto
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "999999")]
    public decimal Quantity { get; set; }

    [Required]
    [StringLength(20)]
    public string Unit { get; set; } = string.Empty;
}
