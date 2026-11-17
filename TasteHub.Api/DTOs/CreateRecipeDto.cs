using System.ComponentModel.DataAnnotations;
using TasteHub.Entities;

namespace TasteHub.Api.DTOs;

public class CreateRecipeDto
{
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Instructions { get; set; } = string.Empty;

    [Range(1, 1440)]
    public int DurationInMinutes { get; set; }

    [EnumDataType(typeof(DifficultyLevel))]
    public DifficultyLevel Difficulty { get; set; }

    [EnumDataType(typeof(RecipeCategory))]
    public RecipeCategory Category { get; set; }

    [Range(1, int.MaxValue)]
    public int UserId { get; set; }
}