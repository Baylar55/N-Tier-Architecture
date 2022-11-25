using Web.ViewModels.Product;

namespace Web.Services.Abstract
{
    public interface IProductService
    {
        Task<ProductIndexVM> GetAllAsync();
        Task<ProductCreateVM> GetCreateModelAsync();
        Task<ProductUpdateVM> GetUpdateModelAsync(int id);
        Task<ProductPhotoUpdateVM> GetUpdateProductPhotoAsync(int id);
        Task<ProductDetailsVM> GetDetailsAsync(int id); 
        Task<bool> CreateAsync(ProductCreateVM model);
        Task<bool> UpdateAsync(ProductUpdateVM model);
        Task<bool> DeleteAsync(int id);
        Task<bool> UpdatePhotoAsync(int id, ProductPhotoUpdateVM model);
        Task<bool> DeletePhotoAsync(int id, ProductPhotoDeleteVM model);
    }
}
