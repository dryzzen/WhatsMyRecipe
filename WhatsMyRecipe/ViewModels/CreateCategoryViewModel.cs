using System.ComponentModel.DataAnnotations;

namespace WhatsMyRecipe.ViewModels
{
    public class CreateCategoryViewModel
    {
        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; }

        [Display(Name = "Category Image")]
        public IFormFile? ImageFile { get; set; }
    }
}
