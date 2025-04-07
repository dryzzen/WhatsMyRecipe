namespace WhatsMyRecipe.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string? Description { get; set; }

        public string Ingredients { get; set; }

        public string Instructions { get; set; }

        public int CategoryId { get; set; }

        public Category Category { get; set; }

        public string? Image { get; set; }

        public string? UserId { get; set; }
    }
}
