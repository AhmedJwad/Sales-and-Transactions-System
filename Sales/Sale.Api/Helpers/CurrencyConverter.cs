

using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;

namespace Sale.Api.Helpers
{
    public class CurrencyConverter : ICurrencyConverter
    {
        private readonly DataContext _context;

        public CurrencyConverter(DataContext context)
        {
            _context = context;
        }
        public async Task<decimal> ConvertFromIQDAsync(decimal amount, string targetCurrency)
        {
            if(targetCurrency.ToUpper() == "IQ")
            {
                return amount;
            }
            var rate = await _context.exchangeRates.Include(r => r.BaseCurrency).Include(r => r.TargetCurrency)
                               .Where(r => r.BaseCurrency.Code == "IQ" && r.TargetCurrency.Code == targetCurrency.ToUpper() && r.IsActive)
                               .Select(r => r.Rate).FirstOrDefaultAsync();

            if(rate<=0)
            {
               return amount ;
            }
            return amount / rate;

        }
    }
}
