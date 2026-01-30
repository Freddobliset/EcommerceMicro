using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.ClientHttp.Interfaces;

namespace Warehouse.ClientHttp
{
    public class WarehouseClient : IWarehouseClient
    {
        private readonly HttpClient _httpClient;

        public WarehouseClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> ProductExistsAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"api/products/{productId}");
            return response.IsSuccessStatusCode;
        }
    }
}
