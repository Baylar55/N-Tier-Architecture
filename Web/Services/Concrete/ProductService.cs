using Core.Entities;
using Core.Utilities.Helpers;
using DataAccess.Contexts;
using DataAccess.Repositories.Abstract;
using DataAccess.Repositories.Concrete;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Web.Services.Abstract;
using Web.ViewModels.Product;

namespace Web.Services.Concrete
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductPhotoRepository _productPhotoRepository;
        private readonly IFileService _fileService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ModelStateDictionary _modelState;

        public ProductService(IProductRepository productRepository,
                              IProductPhotoRepository productPhotoRepository,
                              IActionContextAccessor actionContextAccessor,
                              IFileService fileService,
                              ICategoryRepository categoryRepository)

        {
            _productRepository = productRepository;
            _productPhotoRepository = productPhotoRepository;
            _fileService = fileService;
            _categoryRepository = categoryRepository;
            _modelState = actionContextAccessor.ActionContext.ModelState;
        }

        public async Task<ProductIndexVM> GetAllAsync()
        {
            var model = new ProductIndexVM()
            {
                Products = await _productRepository.GetAllAsync()
            };
            return model;
        }

        public async Task<ProductCreateVM> GetCreateModelAsync()
        {
            var category = await _categoryRepository.GetAllAsync();
            var model = new ProductCreateVM
            {
                Categories = category.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToList()
            };
            return model;
        }

        public async Task<bool> CreateAsync(ProductCreateVM model)
        {
            var category = await _categoryRepository.GetAllAsync();
            model.Categories = category.Select(c => new SelectListItem
            {
                Text = c.Title,
                Value = c.Id.ToString()
            }).ToList();

            bool isExist = await _productRepository.AnyAsync(p => p.Name.ToLower().Trim() == model.Name.ToLower().Trim());
            if (isExist)
            {
                _modelState.AddModelError("Name", "This name is already exist");
                return false;
            }

            if (await _categoryRepository.GetAsync(model.CategoryId) == null)
            {
                _modelState.AddModelError("CategoryId", "This category isn't exist");
                return false;
            }
            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    _modelState.AddModelError("Photo", $"{model.Photo.FileName} should be in image format");
                    return false;
                }
                else if (!_fileService.CheckSize(model.Photo, 400))
                {
                    _modelState.AddModelError("Photo", $"{model.Photo.FileName}'s size sould be smaller than 400kb");
                    return false;
                }
            }

            var product = new Product
            {
                Name = model.Name,
                CategoryId = model.CategoryId,
                Cost = model.Cost,
                Description = model.Description,
                Dimension = model.Dimension,
                CreatedAt = DateTime.Now,
                Quantity = model.Quantity,
                Weight = model.Weight,
                StatusType = model.StatusType,
                MainPhotoName = await _fileService.UploadAsync(model.Photo)
            };
            await _productRepository.CreateAsync(product);
            bool hasError = false;
            foreach (var photo in model.ProductPhotos)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    _modelState.AddModelError("Photo", $"{photo.FileName} should be in image format");
                    hasError = true;
                }
                else if (!_fileService.CheckSize(model.Photo, 400))
                {
                    _modelState.AddModelError("Photo", $"{photo.FileName}'s size sould be smaller than 400kb");
                    hasError = true;
                }
            }
            if (hasError) return hasError;

            int order = 1;
            foreach (var photo in model.ProductPhotos)
            {
                var productPhoto = new ProductPhoto
                {
                    ProductId = product.Id,
                    Name = await _fileService.UploadAsync(photo),
                    Order = order++,
                    CreatedAt = DateTime.Now,
                };
                await _productPhotoRepository.CreateAsync(productPhoto);
            }



            return true;
        }

        public async Task<ProductUpdateVM> GetUpdateModelAsync(int id)
        {
            var category = await _categoryRepository.GetAllAsync();
            
            var product = await _productRepository.GetProductWithProductPhotos(id);

            if (product == null) return null;
            var model = new ProductUpdateVM
            {
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Dimension = product.Dimension,
                StatusType = product.StatusType,
                CategoryId = product.CategoryId,
                Weight = product.Weight,
                Cost = product.Cost,
                ProductPhotos=product.ProductPhotos,
                Categories = category.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToList()

            };
            return model;
        }

        public async Task<bool> UpdateAsync(ProductUpdateVM model)
        {
            var dbProduct = await _productRepository.GetAsync(model.Id);
            if (dbProduct == null) return false;
            dbProduct.Cost = model.Cost;
            dbProduct.Name = model.Name;
            dbProduct.Weight = model.Weight;
            dbProduct.Description = model.Description;
            dbProduct.CategoryId = model.CategoryId;
            dbProduct.Dimension = model.Dimension;
            dbProduct.StatusType = model.StatusType;
            dbProduct.Quantity = model.Quantity;

            if (model.Photo != null)
            {
                if (!_fileService.IsImage(model.Photo))
                {
                    _modelState.AddModelError("Photo", $"{model.Photo.FileName} should be in image format");
                }
                else if (!_fileService.CheckSize(model.Photo, 400))
                {
                    _modelState.AddModelError("Photo", $"{model.Photo.FileName}'s size sould be smaller than 400kb");
                }
                _fileService.Delete(dbProduct.MainPhotoName);
                dbProduct.MainPhotoName = await _fileService.UploadAsync(model.Photo);
            }

            bool hasError = false;

            if (model.ProductPhotos != null)
            {
                foreach (var photo in model.ProductPhotos)
                {
                    if (!_fileService.IsImage(model.Photo))
                    {
                        _modelState.AddModelError("Photo", $"{photo.Name} should be in image format");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(model.Photo, 400))
                    {
                        _modelState.AddModelError("Photo", $"{photo.Name}'s size sould be smaller than 400kb");
                        hasError = true;
                    }
                }
            }
            if (hasError) return hasError;

            if (model.Photos != null)
            {
                foreach (var photo in model.Photos)
                {
                    if (!_fileService.IsImage(photo))
                    {
                        _modelState.AddModelError("Photos", "File must be image");
                        hasError = true;
                    }
                    else if (!_fileService.CheckSize(photo, 400))
                    {
                        _modelState.AddModelError("Photos", $"Photo size must be less 400kb");
                        hasError = true;
                    }

                    int order = 1;
                    var productPhoto = new ProductPhoto
                    {
                        Name = await _fileService.UploadAsync(photo),
                        Order = order,
                        ProductId = dbProduct.Id
                    };
                    order++;

                    await _productPhotoRepository.UpdateAsync(productPhoto);

                }
                if (hasError) return hasError;
            }

            await _productRepository.UpdateAsync(dbProduct);
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetAsync(id);
            if (product != null)
            {
                await _productRepository.DeleteAsync(product);
                return true;
            }
            return false;
        }

        public async Task<ProductDetailsVM> GetDetailsAsync(int id)
        {
            var category = await _categoryRepository.GetAllAsync();
            var product = await _productRepository.GetAsync(id);
            if (product == null) return null;
            var model = new ProductDetailsVM()
            {
                Name = product.Name,
                Description = product.Description,
                Quantity = product.Quantity,
                Dimension = product.Dimension,
                StatusType = product.StatusType,
                CategoryId = product.CategoryId,
                Weight = product.Weight,
                Cost = product.Cost,
                PhotoName = product.MainPhotoName,
                Categories = category.Select(c => new SelectListItem
                {
                    Text = c.Title,
                    Value = c.Id.ToString()
                }).ToList()
            };
            return model;
        }


        #region ProductPhoto

        public async Task<ProductPhotoUpdateVM> GetUpdateProductPhotoAsync(int id)
        {
            var dbProductPhoto = await _productPhotoRepository.GetAsync(id);
            if (dbProductPhoto == null) return null;
            var model = new ProductPhotoUpdateVM
            {
                Id = dbProductPhoto.Id,
                Order = dbProductPhoto.Order,
            };
            return model;
        }

        public async Task<bool> UpdatePhotoAsync(int id, ProductPhotoUpdateVM model)
        {
            if (!_modelState.IsValid) return false;
            var dbProductPhoto = await _productPhotoRepository.GetAsync(id);
            if (dbProductPhoto == null) return false;
            dbProductPhoto.Order = model.Order;
            model.ProductId=dbProductPhoto.ProductId;
            await _productPhotoRepository.UpdateAsync(dbProductPhoto);    
            return true;
        }

        public async Task<bool> DeletePhotoAsync(int id,ProductPhotoDeleteVM model)
        {
            var productPhoto= await _productPhotoRepository.GetAsync(id);
            if (productPhoto == null) return false;
            _fileService.Delete(productPhoto.Name);
            model.ProductId= productPhoto.ProductId;
            await _productPhotoRepository.DeleteAsync(productPhoto);

            return true;
        }

        #endregion
    }
}
