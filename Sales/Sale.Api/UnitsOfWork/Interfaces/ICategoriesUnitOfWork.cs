using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface ICategoriesUnitOfWork
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<CategoryDTO>> GetComboAsync(string lang = "en");
        Task<ActionResponse<Category>> AddFullAsync(CategoryDTO categoryDTO);
        Task<ActionResponse<Category>> UpdateFullAsync(CategoryDTO categoryDTO);
        Task<ActionResponse<Category>> DeleteAsync(int id);
    }
}
