using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class RelatedProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public string? Image { get; set; }
    }
}
