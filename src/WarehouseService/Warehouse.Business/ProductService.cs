using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Warehouse.Business.Interfaces;
using Warehouse.Repository;
using Warehouse.Repository.Entities;
using Warehouse.Shared;

namespace Warehouse.Business
{
    public class ProductService : IProductService
    {
        private readonly WarehouseDbContext _context;

        public ProductService(WarehouseDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
        {
            return await _context.Products
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity
                }).ToListAsync();
        }

        public async Task<int> CreateProductAsync(ProductDto dto)
        {
            var entity = new Product
            {
                Name = dto.Name,
                Price = dto.Price,
                StockQuantity = dto.StockQuantity,
                SupplierName = "Fornitore Default"
            };

            _context.Products.Add(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        public async Task<ProductDto?> GetProductByIdAsync(int id)
        {
            var p = await _context.Products.FindAsync(id);
            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                StockQuantity = p.StockQuantity
            };
        }

        public async Task UpdateStockAsync(int productId, int quantitySold)
        {
            var product = await _context.Products.FindAsync(productId);

            if (product != null)
            {
                product.StockQuantity -= quantitySold;

                await _context.SaveChangesAsync();
            }
        }

    }
}
