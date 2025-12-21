using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Interfaces
{
    public interface IProductsUnitofWork
    {
        Task<IEnumerable<Product>> GetComboAsync(string lang = "en");
        Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination);
        Task<ActionResponse<Product>> DeleteAsync(int id);
        Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO);
        Task<ActionResponse<Product>> GetAsync(int id);
        Task<ActionResponse<IEnumerable<ProductDTO>>> GetAsyncProduct(PaginationDTO pagination);
        Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination);
        Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO);
        Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO);
        Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync(string lang = "en");
        Task<ActionResponse<IEnumerable<ProductDTO>>> GetProductsBySubcategoryAsync(int subcategoryId);
        Task<ActionResponse<IEnumerable<ProductResponseDTO>>> FilterProducts(ProductFilterDto productFilterDto);
        Task<ActionResponse<IEnumerable<ProductResponseDTO>>> GetfullProduct();
        Task<ActionResponse<List<Product>>> GetProductsByIdsAsync(List<int> ids);
        Task<ActionResponse<Product>> UpdateAsync(Product product);
    }
}
