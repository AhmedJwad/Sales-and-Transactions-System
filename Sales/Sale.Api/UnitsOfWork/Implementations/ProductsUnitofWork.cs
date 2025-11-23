using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class ProductsUnitofWork :GenericUnitOfWork<Product>, IProductsUnitofWork
    {
        private readonly IProductRepository _productRepository;

        public ProductsUnitofWork(IGenericRepository<Product> repository, IProductRepository productRepository) : base(repository)
        {
           _productRepository = productRepository;
        }
        public async Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO)
        => await _productRepository.AddFullAsync(productDTO);
        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
       => await _productRepository.AddImageAsync(imageDTO);
        public override async Task<ActionResponse<Product>> DeleteAsync(int id)
        => await _productRepository.DeleteAsync(id);

        
        public override async Task<ActionResponse<Product>> GetAsync(int id)
        => await _productRepository.GetAsync(id);
        public  async Task<ActionResponse<IEnumerable<ProductDTO>>> GetAsync(PaginationDTO pagination)
        => await _productRepository.GetAsync(pagination);
        public async Task<IEnumerable<Product>> GetComboAsync()
        => await _productRepository.GetComboAsync();
        public async Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync(string lang = "en")
        => await _productRepository.GetProductCountByCategoryAsync(lang);
        public async Task<ActionResponse<IEnumerable<ProductDTO>>> GetProductsBySubcategoryAsync(int subcategoryId)
       => await _productRepository.GetProductsBySubcategoryAsync(subcategoryId);
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _productRepository.GetRecordsNumberAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _productRepository.GetTotalPagesAsync(pagination);
        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        => await _productRepository.RemoveLastImageAsync(imageDTO);
        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        => await _productRepository.UpdateFullAsync(productDTO);
        public async Task<ActionResponse<IEnumerable<ProductResponseDTO>>> FilterProducts(ProductFilterDto productFilterDto)
        => await _productRepository.FilterProducts(productFilterDto);
        public async Task<ActionResponse<IEnumerable<ProductResponseDTO>>> GetfullProduct()
        =>  await _productRepository.GetfullProduct();

        public async Task<ActionResponse<List<Product>>> GetProductsByIdsAsync(List<int> ids)
        => await _productRepository.GetProductsByIdsAsync(ids);
    }
}
