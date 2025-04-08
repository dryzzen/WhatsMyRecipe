using System.ComponentModel.DataAnnotations;

namespace WhatsMyRecipe.ViewModels
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Display(Name = "Category Image")]
        public IFormFile? ImageFile { get; set; }
        public string? ExistingImagePath { get; set; }

        [Display(Name = "Recipe Count")]
        public int RecipeCount { get; set; }
    }
}
