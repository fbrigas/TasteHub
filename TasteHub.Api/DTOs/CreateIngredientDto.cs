using System.ComponentModel.DataAnnotations;

namespace TasteHub.Api.DTOs;

public class CreateIngredientDto
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
}