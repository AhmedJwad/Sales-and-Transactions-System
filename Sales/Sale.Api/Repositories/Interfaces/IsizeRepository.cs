using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IsizeRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<Sizep>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Sizep>>> GetAsync();
        Task<ActionResponse<IEnumerable<Sizep>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<Sizep>> GetComboAsync();
    }
}
