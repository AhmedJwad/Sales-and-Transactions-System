using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ProductPrice
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public int CurrencyId { get; set; }
        public Currency? Currency { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cost { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
