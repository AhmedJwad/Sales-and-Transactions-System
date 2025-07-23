using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class SubcategoriesUnitofWorks :GenericUnitOfWork<Subcategory>, iSubcategoriesUnitofWorks
    {
        private readonly ISubcategoryRepository _subcategoryRepository;

        public SubcategoriesUnitofWorks(IGenericRepository<Subcategory> repository, ISubcategoryRepository subcategoryRepository) : base(repository)
        {
           _subcategoryRepository = subcategoryRepository;
        }

        public override async Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination)
        => await _subcategoryRepository.GetAsync(pagination);

        public async Task<IEnumerable<Subcategory>> GetComboAsync()
       => await _subcategoryRepository.GetComboAsync();
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _subcategoryRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _subcategoryRepository.GetTotalPagesAsync(pagination);
    }
}
