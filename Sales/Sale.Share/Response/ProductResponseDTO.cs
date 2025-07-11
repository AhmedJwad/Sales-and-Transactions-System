using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Response
{
    public class ProductResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Barcode { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal DesiredProfit { get; set; }
        public decimal Stock { get; set; }
        public string Brand { get; set; } = "No Brand";
        public bool HasSerial { get; set; }
        public List<string> Subcategories { get; set; } = new();
        public List<string> Categories { get; set; } = new();
        public string? Image { get; set; }
        public int SerialCount { get; set; }
        public int ProductImagesNumber { get; set; }
    }
}
