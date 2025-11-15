using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class brandUnitofWork :GenericUnitOfWork<Brand>, IbrandUnitofWork
    {
        private readonly IbrandRepository _brandRepository;

        public brandUnitofWork(IGenericRepository<Brand> repository , IbrandRepository brandRepository) : base(repository)
        {
          _brandRepository = brandRepository;
        }

        public async Task<ActionResponse<Brand>> AddFullAsync(BrandDTO brandDTO)
         => await _brandRepository.AddFullAsync(brandDTO);

        public override async Task<ActionResponse<IEnumerable<Brand>>> GetAsync(PaginationDTO pagination)
        => await _brandRepository.GetAsync(pagination);

        public async Task<IEnumerable<Brand>> GetComboAsync(int subcategoryId, string lang = "en")
        => await _brandRepository.GetComboAsync(subcategoryId, lang);

        public async Task<IEnumerable<Brand>> GetComboAsync(string lang = "en")
        =>await _brandRepository.GetComboAsync(lang);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _brandRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _brandRepository.GetTotalPagesAsync(pagination);

        public async Task<ActionResponse<Brand>> UpdateFullAsync(BrandDTO brandDTO)
        => await _brandRepository.UpdateFullAsync(brandDTO);
        public override async Task<ActionResponse<Brand>> DeleteAsync(int id)
        => await _brandRepository.DeleteAsync(id);

        public override async Task<ActionResponse<Brand>> GetAsync(int id) 
        => await _brandRepository.GetAsync(id);
    }
}
