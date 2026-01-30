using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Entities
{
    public class OrderItem
    {
        public int Id { get; set; } //id articolo ordine
        public int ProductId { get; set; } //id prodotto del Warehouse
        public int Quantity { get; set; } //quantità ordinata
        public decimal UnitPrice { get; set; } //prezzo unitario

        public int OrderId { get; set; } //id ordine
        public OrderEntity Order { get; set; } = null!; //riferimento all'ordine

    }
}
