using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Shared;
namespace Warehouse.Business.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetAllProductsAsync();
        Task<ProductDto?> GetProductByIdAsync(int id);
        Task<int> CreateProductAsync(ProductDto productDto);
        Task UpdateStockAsync(int productId, int quantitySold);

    }
}
