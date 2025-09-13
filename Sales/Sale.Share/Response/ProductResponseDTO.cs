using Sale.Share.DTOs;
using Sale.Share.Entities;
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
        public string Name { get; set; } = null!;
        public string Barcode { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
        public decimal DesiredProfit { get; set; }
        public decimal Stock { get; set; }
        public int BrandId { get; set; }
        public BrandDTO? brand { get; set; }
        public bool HasSerial { get; set; }
        public List<string>? ProductSubCategories { get; set; }
        public List<string>? ProductColor { get; set; }
        public List<string>? ProductSize { get; set; }
        public List<string>? ProductImages { get; set; }
        public List<string>? SerialNumbers { get; set; }
    }
}
