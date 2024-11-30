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
            var cartItems = _context.CartItems.Include(c => c.Part).ToList();
            var totalAmount = cartItems.Sum(c => c.Quantity * c.Part.Price);

            var model = new Order
            {
                OrderItems = cartItems.Select(item => new OrderItem
                {
                    PartId = item.PartId,
                    Part = item.Part,
                    Quantity = item.Quantity,
                    Price = (decimal)item.Part.Price
                }).ToList(),
                TotalAmount = (decimal)totalAmount
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult SubmitOrder(Order order)
        {
            if (ModelState.IsValid)
            {
                order.OrderDate = DateTime.Now;
                order.UserId = User.Identity.Name;

                _context.Orders.Add(order);
                _context.SaveChanges();

                var cartItems = _context.CartItems.Where(c => c.UserId == order.UserId).ToList();
                _context.CartItems.RemoveRange(cartItems);
                _context.SaveChanges();

                return RedirectToAction("OrderConfirmation", new { orderId = order.Id });
            }

            return View("Checkout", order);
        }

        public IActionResult OrderConfirmation(int orderId)
        {
            var order = _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Part)
                .FirstOrDefault(o => o.Id == orderId);

            return View(order);
        }
    }
}