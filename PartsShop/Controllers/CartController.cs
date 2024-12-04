using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PartsShop.Data;
using PartsShop.Models;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;

namespace PartsShop.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Add to Cart
        [HttpPost]
        public async Task<IActionResult> AddToCart(int partId, int quantity)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var part = await _context.Parts.FindAsync(partId);

            if (!User.Identity.IsAuthenticated)
            {
                return Redirect("https://localhost:7022/Identity/Account/Login"); // Replace redirection if in production
            }

            if (part == null)
            {
                TempData["Error"] = "Part not found!";
                return RedirectToAction("Index");
            }

            var cartItem = await _context.CartItems
                                         .FirstOrDefaultAsync(ci => ci.PartId == partId && ci.UserId == userId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    PartId = partId,
                    Quantity = quantity,
                    UserId = userId,
                    Price = part.Price
                };
                _context.CartItems.Add(cartItem);
                TempData["Message"] = $"Added {part.Name} to cart with quantity {quantity}!";
            }
            else
            {
                cartItem.Quantity += quantity;
                _context.CartItems.Update(cartItem);
                TempData["Message"] = $"Updated quantity of {part.Name} to {cartItem.Quantity}!";
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // View Cart
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.CartItems
                                           .Where(ci => ci.UserId == userId)
                                           .Include(ci => ci.Part)
                                           .ToListAsync();

            var totalAmount = cartItems.Sum(ci => ci.Part.Price * ci.Quantity);

            var cartViewModel = new CartViewModel
            {
                CartItems = cartItems,
                TotalAmount = totalAmount
            };

            return View(cartViewModel);
        }

        // Remove from Cart
        public IActionResult RemoveFromCart(int id)
        {
            var cartItem = _context.CartItems.Find(id);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        public IActionResult Checkout()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cartItems = _context.CartItems
                .Where(ci => ci.UserId == userId && ci.Quantity > 0)
                .Include(ci => ci.Part)
                .ToList();

            var model = new Order
            {
                OrderItems = new List<OrderItem>(),
                TotalAmount = 0
            };

            foreach (var cartItem in cartItems)
            {
                model.OrderItems.Add(new OrderItem
                {
                    PartId = cartItem.PartId,
                    Part = cartItem.Part,
                    Quantity = cartItem.Quantity,
                    Price = cartItem.Part.Price
                });
            }

            model.TotalAmount = model.OrderItems.Sum(oi => oi.Quantity * oi.Price);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitOrder(Order model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var cartItems = await _context.CartItems
                .Where(ci => ci.UserId == userId)
                .Include(ci => ci.Part)
                .ToListAsync();

            if (cartItems == null || cartItems.Count == 0)
            {
                TempData["Error"] = "Your cart is empty or contains no valid items.";
                return RedirectToAction("Index", "Cart");
            }

            var newOrder = new Order
            {
                UserId = userId,
                ShippingAddress = model.ShippingAddress,
                PaymentMethod = model.PaymentMethod,
                OrderItems = cartItems.Select(ci => new OrderItem
                {
                    PartId = ci.PartId,
                    Quantity = ci.Quantity,
                    Price = ci.Part.Price
                }).ToList(),
                TotalAmount = cartItems.Sum(ci => ci.Quantity * ci.Part.Price),
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Your order has been successfully submitted!";
            return RedirectToAction("OrderConfirmation", new { orderId = newOrder.Id });
        }

        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Part)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
            {
                TempData["Error"] = "Order not found.";
                return RedirectToAction("Index", "Home");
            }

            return View(order);
        }
    }
}