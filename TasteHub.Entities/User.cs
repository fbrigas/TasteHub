namespace TasteHub.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = "User";
    public bool IsBlocked { get; set; } = false; // Indicates if the user is blocked
    public List<Recipe> Recipes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
    public List<Favorite> Favorites { get; set; } = new();
    public List<Rating> Ratings { get; set; } = new();

}
