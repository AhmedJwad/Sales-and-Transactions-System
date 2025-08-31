using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class ColourUnitofWork :GenericUnitOfWork<Colour>, IcolourUnitofWorks
    {
        private readonly IcolourRepository _colourRepository;

        public ColourUnitofWork(IGenericRepository<Colour> repository , IcolourRepository colourRepository) : base(repository)
        {
          _colourRepository = colourRepository;
        }

        public override async Task<ActionResponse<Colour>> GetAsync(int id)
        => await _colourRepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<Colour>>> GetAsync()
        => await _colourRepository.GetAsync();
        public override async Task<ActionResponse<IEnumerable<Colour>>> GetAsync(PaginationDTO pagination)
        => await _colourRepository.GetAsync(pagination);
        public async Task<IEnumerable<Colour>> GetComboAsync()
        => await _colourRepository.GetComboAsync();
        public override async  Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _colourRepository.GetRecordsNumberAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _colourRepository.GetTotalPagesAsync(pagination);
    }
}
