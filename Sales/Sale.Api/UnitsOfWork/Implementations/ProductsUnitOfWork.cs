﻿using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class ProductsUnitOfWork : GenericUnitOfWork<Product>, IProductsUnitOfWork
    {
        private readonly IProductsRepository _productsRepository;

        public  ProductsUnitOfWork(IGenericRepository<Product> repository, IProductsRepository productsRepository) : base(repository)
        {
          _productsRepository = productsRepository;
        }

        public async Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO)
       => await _productsRepository.AddFullAsync(productDTO);

        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
       => await _productsRepository.AddImageAsync(imageDTO);

        public override async Task<ActionResponse<Product>> DeleteAsync(int id)
        => await _productsRepository.DeleteAsync(id);

        public override async Task<ActionResponse<Product>> GetAsync(int id)
        => await _productsRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        => await _productsRepository.GetAsync(pagination);

        public async Task<IEnumerable<Product>> GetComboAsync()
        => await _productsRepository.GetComboAsync();

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _productsRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _productsRepository.GetTotalPagesAsync(pagination);

        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        => await _productsRepository.RemoveLastImageAsync(imageDTO);

        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        => await _productsRepository.UpdateFullAsync(productDTO);
    }
}
