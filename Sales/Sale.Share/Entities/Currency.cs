using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class Currency
    {
        public int Id { get; set; }

        [Required, MaxLength(10)]
        public string Code { get; set; } = null!;   // USD, IQD, EUR

        [Required, MaxLength(50)]
        public string? Name { get; set; } = null!;   // US Dollar, Iraqi Dinar
        
        public ICollection<ProductPrice>? ProductPrices { get; set; }
        public ICollection<ExchangeRate>? ExchangeRatesAsBase { get; set; }
        public ICollection<ExchangeRate>? ExchangeRatesAsTarget { get; set; }


    }
}
