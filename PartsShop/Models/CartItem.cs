using Microsoft.AspNetCore.Identity;
namespace PartsShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public virtual Part Part { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
