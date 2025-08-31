using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IcolourUnitofWorks
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<Colour>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Colour>>> GetAsync();
        Task<ActionResponse<IEnumerable<Colour>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<Colour>> GetComboAsync();
    }
}
