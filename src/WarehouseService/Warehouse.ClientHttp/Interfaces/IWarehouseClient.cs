using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.ClientHttp.Interfaces
{
    public interface IWarehouseClient
    {
        Task<bool> ProductExistsAsync(int productId);
    }
}
