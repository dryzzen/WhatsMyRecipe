using WhatsMyRecipe.Models;

namespace WhatsMyRecipe.ViewModels
{
    public class RecipesByCategoryViewModel
    {
        public string CategoryName { get; set; }
        public List<Recipe> Recipes { get; set; }
        public string ImageUrl { get; set; }
    }
}
