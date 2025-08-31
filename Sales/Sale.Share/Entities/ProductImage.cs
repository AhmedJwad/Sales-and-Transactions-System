using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public Product? Product { get; set; }
        public int ProductId { get; set; }
        [Display(Name = "Image")]
        public string Image { get; set; } = null!;        
        public ICollection<ProductColorImage>? productColorImages { get; set; }
    }
}
