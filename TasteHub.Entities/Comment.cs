namespace TasteHub.Entities;

public class Comment
{
    public int Id { get; set; }
    public int RecipeId { get; set; } // Foreign key to the Recipe entity
    public int UserId { get; set; } // Foreign key to the User entity
    public string Content { get; set; } = string.Empty; // Content of the comment
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Timestamp of when the comment was created
    public Recipe Recipe { get; set; } = null!;
    public User User { get; set; } = null!;
}