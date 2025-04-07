using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WhatsMyRecipe.ViewModels
{
    public class CreateRecipeViewModel
    {
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Recipe Image")]
        public IFormFile RecipeImage { get; set; }

        public SelectList Categories { get; set; }
    }
}
