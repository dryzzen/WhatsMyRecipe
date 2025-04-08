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

        public int CategoryId { get; set; }

        public string? Image { get; set; }
        public string FormattedIngredients => Ingredients?.Replace("\n", "<br/>");
        public string FormattedInstructions => Instructions?.Replace("\n", "<br/>");

    }
}
