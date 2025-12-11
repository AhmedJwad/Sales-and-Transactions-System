using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class ProductPriceDTO
    {
        public string CurrencyCode { get; set; } = null!;
        public decimal Price { get; set; }
        public decimal Cost { get; set; }
    }
}
