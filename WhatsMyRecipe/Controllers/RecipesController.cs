using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WhatsMyRecipe.Data;
using WhatsMyRecipe.Models;
using WhatsMyRecipe.ViewModels;
using Microsoft.EntityFrameworkCore;
namespace WhatsMyRecipe.Controllers
{
    [Authorize]
    public class RecipesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public RecipesController(ApplicationDbContext context,UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Recipes
        public async Task<IActionResult> Index(int categoryId)
        {
            var userId = _userManager.GetUserId(User);

            var recipes = await _context.Recipes
                .Where(r => r.CategoryId == categoryId && r.UserId == userId)
                .Include(r => r.Category)
                .ToListAsync();

            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId);

            if (category == null)
            {
                return NotFound();
            }

            ViewBag.CategoryName = category.Name;
            ViewBag.CategoryImage = category.Image ?? "/images/default-category.jpg";

            return View(recipes);
        }

        // GET: Recipes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var recipe = await _context.Recipes
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (recipe == null) return NotFound();
            if (recipe.UserId != _userManager.GetUserId(User)) return Forbid();

            var model = new RecipeDetailsViewModel
            {
                Id = recipe.Id,
                Title = recipe.Title,
                Description = recipe.Description,
                Ingredients = recipe.Ingredients,
                Instructions = recipe.Instructions,
                CategoryName = recipe.Category?.Name ?? "Uncategorized",
                ImagePath = recipe.Image,
                FormattedIngredients = FormatText(recipe.Ingredients),
                FormattedInstructions = FormatText(recipe.Instructions)
            };

            return View(model);
        }

        private string FormatText(string input)
        {
            // Add your formatting logic here (e.g., replace commas with line breaks)
            return input;
        }



        [HttpGet]
        public async Task<IActionResult> CreateRecipe()
        {
            var userId = _userManager.GetUserId(User);
            var categories = await _context.Categories
                .Where(c => !c.IsCustom || c.UserId == userId)
                .ToListAsync();

            var model = new CreateRecipeViewModel
            {
                Categories = new SelectList(categories, "Id", "Name")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRecipe(CreateRecipeViewModel model)
        {
            var userId = _userManager.GetUserId(User);
            var categories = await _context.Categories
                .Where(c => !c.IsCustom || c.UserId == userId)
                .ToListAsync();
            model.Categories = new SelectList(categories, "Id", "Name");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var recipe = new Recipe
            {
                Title = model.Title,
                Description = model.Description,
                Ingredients = model.Ingredients,
                Instructions = model.Instructions,
                CategoryId = model.CategoryId,
                UserId = userId,
            };

            // Handle image upload
            if (model.RecipeImage != null)
            {
                try
                {
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(model.RecipeImage.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("RecipeImage", "Only JPG, PNG or GIF images are allowed");
                        return View(model);
                    }

                    var uploadsFolder = Path.Combine("wwwroot", "images", "recipes");
                    Directory.CreateDirectory(uploadsFolder);

                    var fileName = $"{Guid.NewGuid()}{fileExtension}";
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.RecipeImage.CopyToAsync(stream);
                    }

                    recipe.Image = $"/images/recipes/{fileName}";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Image upload failed: {ex.Message}");
                    ModelState.AddModelError("RecipeImage", "Error uploading image");
                    return View(model);
                }
            }

            try
            {
                _context.Recipes.Add(recipe);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "Recipes", new { categoryId = recipe.CategoryId });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving recipe: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the recipe.");
                return View(model);
            }
        }



        // GET: Recipes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe == null)
            {
                return NotFound();
            }
            return View(recipe);
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Ingredients,Instructions,Category,Image,UserId")] Recipe recipe)
        {
            if (id != recipe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(recipe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RecipeExists(recipe.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(recipe);
        }

        // GET: Recipes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var recipe = await _context.Recipes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (recipe == null)
            {
                return NotFound();
            }

            return View(recipe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var recipe = await _context.Recipes.FindAsync(id);
            if (recipe != null)
            {
                _context.Recipes.Remove(recipe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RecipeExists(int id)
        {
            return _context.Recipes.Any(e => e.Id == id);
        }


        private async Task<string?> HandleImageUpload(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0)
                return null;

            var uploadsFolder = Path.Combine("wwwroot", "images", "categories");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return "/images/categories/" + uniqueFileName;
        }

    }



}
