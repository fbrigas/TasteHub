namespace TasteHub.Entities;

public class Rating
{
    public int Id { get; set; }
    public int RecipeId { get; set; } // Foreign key to the Recipe entity
    public int UserId { get; set; } // Foreign key to the User entity
    public int Value { get; set; } // Rating value (e.g., 1-5)
    public Recipe Recipe { get; set; } = null!;
    public User User { get; set; } = null!;
}