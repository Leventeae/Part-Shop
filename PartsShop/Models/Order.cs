using System.ComponentModel.DataAnnotations;

namespace PartsShop.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        [Required(ErrorMessage = "Shipping address is required.")]
        public string ShippingAddress { get; set; }
        [Required(ErrorMessage = "Payment method is required.")]
        public string PaymentMethod { get; set; }
        public decimal TotalAmount { get; set; }
        public List<OrderItem> OrderItems { get; set; }
        public string Status { get; set; }
    }

    public class OrderItem
    {
        public int Id { get; set; }
        public int PartId { get; set; }
        public Part Part { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    public class OrderHistoryViewModel
    {
        public List<Order> Orders { get; set; }
    }
}