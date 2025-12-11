namespace Sale.Api.Helpers
{
    public interface ICurrencyConverter
    {
        Task<decimal> ConvertFromIQDAsync(decimal amount, string targetCurrency);
    }
}
