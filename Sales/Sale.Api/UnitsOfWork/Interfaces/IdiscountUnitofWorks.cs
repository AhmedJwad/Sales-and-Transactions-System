using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IdiscountUnitofWorks
    {
        Task<IEnumerable<Discount>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Discount>> DeleteAsync(int id);
        Task<ActionResponse<Discount>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Discount>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Discount>> AddFullAsync(DiscountDTO discountDTO);
        Task<ActionResponse<Discount>> UpdateFullAsync(DiscountDTO discountDTO);
    }
}
