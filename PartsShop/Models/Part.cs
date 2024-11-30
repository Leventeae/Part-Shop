using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace PartsShop.Models
{
    public class Part
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        public string ImgUrl { get; set; }
    }
}
