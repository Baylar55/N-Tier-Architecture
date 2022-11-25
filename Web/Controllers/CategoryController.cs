using DataAccess.Contexts;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstract;
using Web.ViewModels.Category;

namespace Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _categoryService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            var isSucceeded = await _categoryService.CreateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = await _categoryService.GetUpdateModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(int id, CategoryUpdateVM model)
        {
            if (!ModelState.IsValid) return View(model);
            if (id != model.Id) return NotFound();


            var isSucceeded = await _categoryService.UpdateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSucceeded = await _categoryService.DeleteAsync(id);
            if(isSucceeded) return RedirectToAction(nameof(Index));
            return NotFound();
        }
    }
}
