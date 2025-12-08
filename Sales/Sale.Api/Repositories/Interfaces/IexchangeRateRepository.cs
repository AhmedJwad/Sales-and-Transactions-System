using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IexchangeRateRepository
    {
        Task<IEnumerable<ExchangeRate>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);     
        Task<ActionResponse<ExchangeRate>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<ExchangeRate>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<ExchangeRate>> AddFullAsync(ExchangeRateDTO exchangeRateDTO);      
       
    }
}
