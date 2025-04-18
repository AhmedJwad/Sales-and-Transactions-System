﻿using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class CategoriesUnitOfWork :GenericUnitOfWork<Category> ,ICategoriesUnitOfWork
    {
        private readonly ICategoriesRepository _categoriesRepository;

        public CategoriesUnitOfWork(IGenericRepository<Category> repository , ICategoriesRepository categoriesRepository) : base(repository)
        {
           _categoriesRepository = categoriesRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination)
       =>await _categoriesRepository.GetAsync(pagination);

        public async Task<IEnumerable<Category>> GetComboAsync()
        => await _categoriesRepository.GetComboAsync();

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _categoriesRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _categoriesRepository.GetTotalPagesAsync(pagination);
    }
}
