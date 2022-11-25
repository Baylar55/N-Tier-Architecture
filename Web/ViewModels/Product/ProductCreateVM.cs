using Core.Constants;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.ViewModels.Product
{
    public class ProductCreateVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IFormFile Photo { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public List<SelectListItem>? Categories { get; set; }
        public int CategoryId { get; set; }  //nullable
        public Status StatusType { get; set; }
        public List<IFormFile> ProductPhotos { get; set; }
    }
}
