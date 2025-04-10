﻿using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Required(ErrorMessage = "Please select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Display(Name = "Recipe Image")]
        public IFormFile RecipeImage { get; set; }

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
