using TasteHub.Entities;

namespace TasteHub.Api.DTOs;

public class CreateRecipeDto
{
    public string Title { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string Instructions { get; set; } = string.Empty;

    public int DurationInMinutes { get; set; }

    public DifficultyLevel Difficulty { get; set; }

    public RecipeCategory Category { get; set; }

    public int UserId { get; set; }
}