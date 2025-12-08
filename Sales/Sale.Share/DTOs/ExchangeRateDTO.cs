using Sale.Share.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.DTOs
{
    public class ExchangeRateDTO
    {
        public int Id { get; set; }
        public int BaseCurrencyId { get; set; }       
        public int TargetCurrencyId { get; set; }       
        public decimal Rate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
