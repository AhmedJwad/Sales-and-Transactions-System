using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IOrdersRepository
    {
        Task<ActionResponse<IEnumerable<OrderResponseDTO>>> GetAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(string email, PaginationDTO pagination);
        Task<ActionResponse<OrderResponseDTO>> GetAsync(int id);
        Task<ActionResponse<Order>> UpdateFullAsync(string email, OrderDTO orderDTO);
        Task<ActionResponse<IEnumerable<Order>>> GetReportAsync(DatesDTO datesDTO);
    }
}
