using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IProductsUnitofWork
    {
        Task<IEnumerable<Product>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<Product>> DeleteAsync(int id);
        Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<Product>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO);
        Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO);
        Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync();
    }
}
