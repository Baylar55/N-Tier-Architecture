using Core.Entities;
using DataAccess.Repositories.Abstract;
using Microsoft.AspNetCore.Mvc;
using Web.Services.Abstract;
using Web.Services.Concrete;
using Web.ViewModels.Product;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _productService.GetAllAsync();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = await _productService.GetCreateModelAsync();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                model = await _productService.GetCreateModelAsync();//bunu redirectin altina at
                return View(model);
            }
            var isSucceeded = await _productService.CreateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var model = await _productService.GetUpdateModelAsync(id);
            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Update(ProductUpdateVM model, int id)
        {
            
            if (id != model.Id) return BadRequest();
            var isSucceeded = await _productService.UpdateAsync(model);
            if (isSucceeded) return RedirectToAction(nameof(Index));
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var isSucceeded = await _productService.DeleteAsync(id);
            if (isSucceeded) return RedirectToAction(nameof(Index));
            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _productService.GetDetailsAsync(id);
            if (model != null) return View(model);
            return NotFound();
        }

        #region ProductPhoto

        [HttpGet]
        public async Task<IActionResult> UpdatePhoto(int id)
        {
            var model = await _productService.GetUpdateProductPhotoAsync(id);
            if (model != null) return View(model);
            return NotFound();
        }



        [HttpPost]
        public async Task<IActionResult> UpdatePhoto(int id, ProductPhotoUpdateVM model)
        {
            if (id != model.Id) return BadRequest();
            var isSucceeded = await _productService.UpdatePhotoAsync(id, model);
            if (!isSucceeded) return NotFound();
            return RedirectToAction("update", "product", new { id = model.ProductId });
        }

        public async Task<IActionResult> DeletePhoto(int id,ProductPhotoDeleteVM model)
        {
            var isSucceeded = await _productService.DeletePhotoAsync(id,model);
            if(!isSucceeded) return NotFound();
            return RedirectToAction("update","product",new {id=model.ProductId});
        }
        #endregion
    }
}
