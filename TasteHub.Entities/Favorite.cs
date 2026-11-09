namespace TasteHub.Entities;

    public class Favorite
    {
        public int Id { get; set; }
        public int UserId { get; set; } // Foreign key to the User entity
        public int RecipeId { get; set; } // Foreign key to the Recipe entity
        public User User { get; set; } = null!;
        public Recipe Recipe { get; set; } = null!;
    }