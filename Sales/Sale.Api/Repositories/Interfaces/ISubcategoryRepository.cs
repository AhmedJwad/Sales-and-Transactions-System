using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface ISubcategoryRepository
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<Subcategory>> GetComboAsync();
        Task<IEnumerable<SubcategoryDTO>> GetComboAsync(int categoryId);
        Task<ActionResponse<Subcategory>> AddFullAsync(SubcategoryDTO subcategoryDTO);
        Task<ActionResponse<Subcategory>> UpdateFullAsync(SubcategoryDTO subcategoryDTO);
        Task<ActionResponse<Subcategory>> DeleteAsync(int id);
    }
}
