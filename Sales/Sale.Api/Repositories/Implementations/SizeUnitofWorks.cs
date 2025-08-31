using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Implementations;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class SizeUnitofWorks :GenericUnitOfWork<Sizep>, IsizeUnitofWorks
    {
        private readonly IsizeRepository _sizeRepository;

        public SizeUnitofWorks(IGenericRepository<Sizep> repository, IsizeRepository sizeRepository) : base(repository)
        {
           _sizeRepository = sizeRepository;
        }

        public override async Task<ActionResponse<Sizep>> GetAsync(int id)
        => await _sizeRepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<Sizep>>> GetAsync()
        => await _sizeRepository.GetAsync();
        public override async Task<ActionResponse<IEnumerable<Sizep>>> GetAsync(PaginationDTO pagination)
        => await _sizeRepository.GetAsync(pagination);
        public async Task<IEnumerable<Sizep>> GetComboAsync()
        => await _sizeRepository.GetComboAsync();
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _sizeRepository.GetRecordsNumberAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _sizeRepository.GetTotalPagesAsync(pagination);
    }
}
