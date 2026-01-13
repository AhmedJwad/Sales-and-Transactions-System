using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class ProductDetailsDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal OldPrice { get; set; }
        public decimal DiscountPercent { get; set; }

        public List<string> Images { get; set; } = new();
        public BrandDTO? Brand { get; set; }

        public List<ColorDTO> Colors { get; set; } = new();
        public List<SizeDTO> Sizes { get; set; } = new();
        public List<SubcategoryDTO> Categories { get; set; } = new();
    }
}
