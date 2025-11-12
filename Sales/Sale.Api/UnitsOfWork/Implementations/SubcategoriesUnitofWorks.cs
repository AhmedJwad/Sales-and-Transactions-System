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

        public async Task<IEnumerable<SubcategoryDTO>> GetComboAsync(string lang = "en")
       => await _subcategoryRepository.GetComboAsync(lang);

        public async Task<IEnumerable<SubcategoryDTO>> GetComboAsync(int categoryId, string lang = "en")
        =>await _subcategoryRepository.GetComboAsync(categoryId , lang);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _subcategoryRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<Subcategory>> GetAsync(int id)
            => await _subcategoryRepository.GetAsync(id);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _subcategoryRepository.GetTotalPagesAsync(pagination);
        public async Task<ActionResponse<Subcategory>> AddFullAsync(SubcategoryDTO subcategoryDTO)
        => await _subcategoryRepository.AddFullAsync(subcategoryDTO);
        public async Task<ActionResponse<Subcategory>> UpdateFullAsync(SubcategoryDTO subcategoryDTO)
        => await _subcategoryRepository.UpdateFullAsync(subcategoryDTO);
        public override async Task<ActionResponse<Subcategory>> DeleteAsync(int Id)
        => await _subcategoryRepository.DeleteAsync(Id);
    }
}
