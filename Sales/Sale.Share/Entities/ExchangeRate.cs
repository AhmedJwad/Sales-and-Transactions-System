using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sale.Share.Entities
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public int BaseCurrencyId { get; set; }
        public Currency BaseCurrency { get; set; } = null!;
        public int TargetCurrencyId { get; set; }
        public Currency TargetCurrency { get; set; } = null!;
        public decimal Rate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime? EndDate { get; set; }
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
