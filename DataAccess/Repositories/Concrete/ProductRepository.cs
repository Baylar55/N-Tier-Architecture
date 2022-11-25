using Core.Entities;
using DataAccess.Contexts;
using DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Concrete
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly AppDbContext _context;

        public ProductRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllWithCategoriesAsync()
        {
            return await _context.Products
                            .Include(p=>p.Category)
                            .ToListAsync();
        }

        public async Task<Product> GetProductWithProductPhotos(int id)
        {
            return await _context.Products
                                 .Include(p => p.ProductPhotos)
                                 .FirstOrDefaultAsync(p => p.Id == id);
        }
    }
}
