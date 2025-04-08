using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WhatsMyRecipe.Data;
using WhatsMyRecipe.Models;
using WhatsMyRecipe.Services;
using WhatsMyRecipe.ViewModels;

namespace WhatsMyRecipe.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileService _fileService;

        public CategoryController( ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IFileService fileService)
        {
            _context = context;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.Identity?.IsAuthenticated == true
                ? _userManager.GetUserId(User)
                : null;

            var categories = await _context.Categories
                .Select(c => new CategoryViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Image = c.Image,
                    IsCustom = c.IsCustom,
                    IsOwner = c.UserId == userId,
                    RecipeCount = c.Recipes.Count
                })
                .ToListAsync();

            ViewBag.IsAuthenticated = User.Identity?.IsAuthenticated ?? false;
            return View(categories);
        }


        public IActionResult CreateCategory()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(CreateCategoryViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var imagePath = await HandleImageUpload(model.ImageFile);

                var category = new Category
                {
                    Name = model.Name,
                    IsCustom = true,
                    UserId = userId,
                    Image = imagePath
                };

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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

        [HttpGet]
        public async Task<IActionResult> EditCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null || (category.IsCustom && category.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            var model = new EditCategoryViewModel
            {
                Id = category.Id,
                Name = category.Name,
                ExistingImagePath = category.Image,
                RecipeCount = await _context.Recipes.CountAsync(r => r.CategoryId == id)
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCategory(EditCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var category = await _context.Categories.FindAsync(model.Id);
            if (category == null || (category.IsCustom && category.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            category.Name = model.Name;

            if (model.ImageFile != null)
            {
                if (!string.IsNullOrEmpty(category.Image))
                {
                    _fileService.DeleteImage(category.Image);
                }
                //for new image upload
                category.Image = _fileService.UploadImage(model.ImageFile, "categories");
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || (category.IsCustom && category.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("DeleteCategory")] 
        public async Task<IActionResult> DeleteCategoryConfirmed(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Recipes)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null || (category.IsCustom && category.UserId != _userManager.GetUserId(User)))
            {
                return NotFound();
            }

            if (category.Recipes.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete category with existing recipes. Move or delete recipes first.";
                return RedirectToAction(nameof(DeleteCategory), new { id });
            }

            if (!string.IsNullOrEmpty(category.Image))
            {
                _fileService.DeleteImage(category.Image);
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
