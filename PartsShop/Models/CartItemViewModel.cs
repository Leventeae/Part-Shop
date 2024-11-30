using PartsShop.Models;
using PartsShop.Data;

namespace PartsShop.Models
{
    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public Part Part { get; set; }
    }
}