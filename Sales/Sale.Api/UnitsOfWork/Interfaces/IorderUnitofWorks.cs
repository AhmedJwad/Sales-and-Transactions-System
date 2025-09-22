using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IorderUnitofWorks
    {
        Task<ActionResponse<IEnumerable<Order>>> GetAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<Order>> GetAsync(int id);
        Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO);
        Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO);
        Task<ActionResponse<Order>> AddAsync(Order order);
    }
}
