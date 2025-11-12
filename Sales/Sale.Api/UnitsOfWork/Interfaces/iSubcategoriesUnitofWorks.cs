using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface iSubcategoriesUnitofWorks
    {
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<Subcategory>> GetAsync(int id);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<IEnumerable<SubcategoryDTO>> GetComboAsync(string lang = "en");
        Task<IEnumerable<SubcategoryDTO>> GetComboAsync(int categoryId, string lang = "en");
        Task<ActionResponse<Subcategory>> AddFullAsync(SubcategoryDTO subcategoryDTO);
        Task<ActionResponse<Subcategory>> UpdateFullAsync(SubcategoryDTO subcategoryDTO);
        Task<ActionResponse<Subcategory>> DeleteAsync(int id);
    }
}
