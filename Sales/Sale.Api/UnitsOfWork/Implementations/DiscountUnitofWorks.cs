using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class DiscountUnitofWorks :GenericUnitOfWork<Discount>, IdiscountUnitofWorks
    {
        private readonly IdiscountRepository _discountRepository;

        public DiscountUnitofWorks(IGenericRepository<Discount> repository , IdiscountRepository discountRepository) : base(repository)
        {
            _discountRepository = discountRepository;
        }

        public async Task<ActionResponse<Discount>> AddFullAsync(DiscountDTO discountDTO)
        =>await _discountRepository.AddFullAsync(discountDTO);
        public override async Task<ActionResponse<Discount>> DeleteAsync(int id)
        => await _discountRepository.DeleteAsync(id);
        public override async Task<ActionResponse<Discount>> GetAsync(int id)
        => await _discountRepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<Discount>>> GetAsync(PaginationDTO pagination)
        => await _discountRepository.GetAsync(pagination);
        public async Task<IEnumerable<Discount>> GetComboAsync()
        => await _discountRepository.GetComboAsync();
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        => await _discountRepository.GetRecordsNumberAsync(paginationDTO);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        => await _discountRepository.GetTotalPagesAsync(paginationDTO);
        public  async Task<ActionResponse<Discount>> UpdateFullAsync(DiscountDTO discountDTO)
        => await _discountRepository.UpdateFullAsync(discountDTO);
    }
}
