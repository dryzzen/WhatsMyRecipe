namespace WhatsMyRecipe.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public bool IsCustom { get; set; }
        public bool IsOwner { get; set; }
        public int RecipeCount { get; set; }
    }
}