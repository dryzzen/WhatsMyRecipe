using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WhatsMyRecipe.Data;
using WhatsMyRecipe.Models;
using WhatsMyRecipe.ViewModels;
using WhatsMyRecipe.Services;

namespace WhatsMyRecipe.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileService _fileService;

        public RecipesController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        private async Task<SelectList> GetUserCategoriesSelectList()
        {
            var userId = _userManager.GetUserId(User);
            var categories = await _context.Categories
                .Where(c => !c.IsCustom || c.UserId == userId)
                .ToListAsync();
            return new SelectList(categories, "Id", "Name");
        }

        public async Task<IActionResult> Index(int categoryId)
        {
            var userId = _userManager.GetUserId(User);
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null) return NotFound();

            var recipes = await _context.Recipes
                .Where(r => r.CategoryId == categoryId && r.UserId == userId)
                .Include(r => r.Category)
                .ToListAsync();

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryImage = category.Image ?? "/images/default-category.jpg";

            return View(recipes);
        }

        public async Task<IActionResult> DetailsRecipe(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null || recipe.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(new RecipeDetailsViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                CategoryName = recipe.Category?.Name ?? "Uncategorized",
                CategoryId = recipe.CategoryId,
                Image = recipe.Image
            });
        }

        [HttpGet]
        public async Task<IActionResult> CreateRecipe()
        {
            return View(new CreateRecipeViewModel
            {
                Categories = await GetUserCategoriesSelectList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecipe(CreateRecipeViewModel model)
        {
            model.Categories = await GetUserCategoriesSelectList();
            ModelState.Remove("Categories");

            if (!ModelState.IsValid) return View(model);

            var recipe = new Recipe
            {
                Title = model.Title,
                Description = model.Description,
                Ingredients = model.Ingredients,
                Instructions = model.Instructions,
                CategoryId = model.CategoryId,
                UserId = _userManager.GetUserId(User)
            };

            if (model.RecipeImage != null)
            {
                try
                {
                    recipe.Image = _fileService.UploadImage(model.RecipeImage, "recipes");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("RecipeImage", ex.Message);
                    return View(model);
                }
            }

            _context.Recipes.Add(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsRecipe", new { id = recipe.Id });
        }

        [HttpGet]
        public async Task<IActionResult> EditRecipe(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null || recipe.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(new EditRecipeViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                CategoryId = recipe.CategoryId,
                CurrentImagePath = recipe.Image,
                Categories = await GetUserCategoriesSelectList()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRecipe(EditRecipeViewModel model)
        {
            model.Categories = await GetUserCategoriesSelectList();
            ModelState.Remove("Categories");
            ModelState.Remove("CurrentImagePath");

            if (!ModelState.IsValid) return View(model);

            var recipe = await _context.Recipes.FindAsync(model.Id);
            if (recipe == null) return NotFound();

            recipe.Title = model.Title;
            recipe.Description = model.Description;
            recipe.Ingredients = model.Ingredients;
            recipe.Instructions = model.Instructions;
            recipe.CategoryId = model.CategoryId;

            if (model.NewImage != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(recipe.Image))
                        _fileService.DeleteImage(recipe.Image);

                    recipe.Image = _fileService.UploadImage(model.NewImage, "recipes");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("NewImage", ex.Message);
                    return View(model);
                }
            }

            _context.Update(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction("DetailsRecipe", new { id = recipe.Id });
        }

        [HttpGet]
        public async Task<IActionResult> DeleteRecipe(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (recipe == null || recipe.UserId != _userManager.GetUserId(User))
                return NotFound();

            return View(recipe);
        }

        [HttpPost, ActionName("DeleteRecipe")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteRecipeConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null) return NotFound();

            if (!string.IsNullOrEmpty(recipe.Image))
                _fileService.DeleteImage(recipe.Image);

            _context.Recipes.Remove(recipe);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Recipes", new { categoryId = recipe.CategoryId });
        }
    }
}