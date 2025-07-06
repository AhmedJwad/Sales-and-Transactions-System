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

        public override async Task<ActionResponse<IEnumerable<Brand>>> GetAsync(PaginationDTO pagination)
        => await _brandRepository.GetAsync(pagination);

        public async Task<IEnumerable<Brand>> GetComboAsync(int subcategoryId)
        => await _brandRepository.GetComboAsync(subcategoryId);

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _brandRepository.GetRecordsNumberAsync(pagination);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _brandRepository.GetTotalPagesAsync(pagination);
    }
}
