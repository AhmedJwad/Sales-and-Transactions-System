using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetComboAsync();
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Product>> DeleteAsync(int id);
        Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<Product>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination);        
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO);
        Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO);
        Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO);
        Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync();
        Task<ActionResponse<IEnumerable<ProductDTO>>> GetProductsBySubcategoryAsync(int subcategoryId);
        Task<ActionResponse<IEnumerable<ProductResponseDTO>>> FilterProducts(ProductFilterDto productFilterDto);
        Task<ActionResponse<IEnumerable<ProductResponseDTO>>> GetfullProduct();
        Task<ActionResponse<List<Product>>> GetProductsByIdsAsync(List<int> ids);
    }
}
