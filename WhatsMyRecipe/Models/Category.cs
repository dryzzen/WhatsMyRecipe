using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace WhatsMyRecipe.Models
{
    public class Category
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool IsCustom    { get; set; }

        public string? UserId { get; set; }
        public string? Image { get; set; }


        public ICollection<Recipe> Recipes { get; set; }

        [ForeignKey("UserId")]
        public virtual IdentityUser? User { get; set; }
    }
}
