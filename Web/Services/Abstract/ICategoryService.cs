using Core.Entities;
using Web.ViewModels.Category;

namespace Web.Services.Abstract
{
    public interface ICategoryService
    {
        Task<CategoryIndexVM> GetAllAsync();
        Task<CategoryUpdateVM> GetUpdateModelAsync(int id);
        Task<bool> CreateAsync(CategoryCreateVM model);
        Task<bool> UpdateAsync(CategoryUpdateVM model);
        Task<bool> DeleteAsync(int id);
    }
}
