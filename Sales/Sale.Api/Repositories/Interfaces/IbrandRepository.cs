using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IbrandRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Brand>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<Brand>> GetAsync(int id);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<Brand>> GetComboAsync(int subcategoryId, string lang = "en");
        Task<IEnumerable<Brand>> GetComboAsync(string lang = "en");
        Task<ActionResponse<Brand>> AddFullAsync(BrandDTO brandDTO);
        Task<ActionResponse<Brand>> UpdateFullAsync(BrandDTO brandDTO);
        Task<ActionResponse<Brand>> DeleteAsync(int id);
    }
}
