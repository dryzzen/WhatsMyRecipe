using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace WhatsMyRecipe.ViewModels
{
    public class EditRecipeViewModel
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string Ingredients { get; set; }

        [Required]
        public string Instructions { get; set; }

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Change Photo")]
        public IFormFile? NewImage { get; set; }

        public string CurrentImagePath { get; set; }

        public SelectList Categories { get; set; }
    }
}
