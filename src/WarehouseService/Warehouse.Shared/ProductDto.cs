using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Shared
{
    public class ProductDto
    {
        public int Id { get; set; } // id prodotto
        public string Name { get; set; } = string.Empty; // nome prodotto
        public decimal Price { get; set; } // prezzo prodotto
        public int StockQuantity { get; set; } // quantità in magazzino
    }
}
