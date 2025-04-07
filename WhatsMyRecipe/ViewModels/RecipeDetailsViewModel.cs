namespace WhatsMyRecipe.ViewModels
{
    public class RecipeDetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Instructions { get; set; }
        public string CategoryName { get; set; }
        public string? ImagePath { get; set; }
        public string? FormattedIngredients { get; set; }
        public string? FormattedInstructions { get; set; }
    }
}
