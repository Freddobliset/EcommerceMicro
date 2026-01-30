using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Order.Repository.Entities
{
    public class OrderEntity
    {
        public int Id { get; set; } //id ordine
        public DateTime OrderDate { get; set; } //data ordine
        public string CustomerEmail { get; set; } = string.Empty; //email cliente
        public decimal TotalAmount { get; set; } //importo totale
        public string Status { get; set; } = "Pending"; //stato ordine
        public List<OrderItem> OrderItems { get; set; } = new(); //lista articoli ordine

    }
}
