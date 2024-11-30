using Microsoft.AspNetCore.Identity;
namespace PartsShop.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public virtual Part Part { get; set; }  // Navigation property to Part
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string UserId { get; set; } // This references the Id in AspNetUsers (the user who owns the cart item)
        public virtual IdentityUser User { get; set; }
    }
}
