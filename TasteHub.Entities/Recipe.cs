namespace TasteHub.Entities;

public class Recipe
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<RecipeIngredient> Ingredients { get; set; } = new();
    public string Instructions { get; set; } = string.Empty;
    public int DurationInMinutes { get; set; } = 0; // Duration in minutes
    public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Easy; // Difficulty level of the recipe
    public int UserId { get; set; } // Foreign key to the User entity
    public User User { get; set; } = null!; // Navigation property to the User entity
    public RecipeStatus Status { get; set; } = RecipeStatus.Draft; // Status of the recipe
    public List<Comment> Comments { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
    public List<Rating> Ratings { get; set; } = new();

}

