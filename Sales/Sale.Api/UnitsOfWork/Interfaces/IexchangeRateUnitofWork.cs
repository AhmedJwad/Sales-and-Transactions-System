using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IexchangeRateUnitofWork
    {
        Task<IEnumerable<Currency>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<ExchangeRate>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<ExchangeRate>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<ExchangeRate>> AddFullAsync(ExchangeRateDTO exchangeRateDTO);
    }
}
