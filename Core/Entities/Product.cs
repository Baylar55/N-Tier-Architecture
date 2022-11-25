using Core.Constants;
using Core.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string MainPhotoName { get; set; }
        public double Cost { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Weight { get; set; }
        public string Dimension { get; set; }
        public Category Category { get; set; }
        public int CategoryId { get; set; }
        public Status StatusType { get; set; }
        public List<ProductPhoto> ProductPhotos { get; set; }
    }
}
