namespace PartsShop.Models
{
    public class CartViewModel
    {
        public List<CartItem> CartItems { get; set; }
        public int CartItemCount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}