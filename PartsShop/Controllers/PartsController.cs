using Microsoft.AspNetCore.Mvc;
using PartsShop.Models;
using PartsShop;
using PartsShop.Data;
using System.Linq;
using System.Security.Claims;
using System.Drawing.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace PartsShop.Controllers
{
    public class PartsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            ViewData["SearchQuery"] = searchQuery;

            if (_context.Parts == null)
            {
                return NotFound();
            }

            var parts = from p in _context.Parts select p;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                parts = parts.Where(p => p.Name.ToLower().Contains(searchQuery.ToLower()));
            }

            var partList = await parts.ToListAsync();

            return View(partList);
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var part = await _context.Parts.FirstOrDefaultAsync(p => p.Id == id);

            if (part == null)
            {
                return NotFound();
            }

            return View(part);
        }

    }
}
