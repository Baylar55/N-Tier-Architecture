using Core.Constants;
using Core.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels.Product
{
    public class ProductUpdateVM
    {
        public ProductUpdateVM()
        {
            ProductPhotos= new List<ProductPhoto>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile? Photo { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int CategoryId { get; set; }
        public Status StatusType { get; set; }
        public List<ProductPhoto>? ProductPhotos { get; set; }
        public List<IFormFile>? Photos { get; set; }
    }
}
