using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class StatesUnitOfWork :GenericUnitOfWork<State>, IStatesUnitOfWork
    {
        private readonly IStatesRepository _staterepository;

        public StatesUnitOfWork(IGenericRepository<State> repository, IStatesRepository staterepository) : base(repository)
        {
           _staterepository = staterepository;
        }

        public override async Task<ActionResponse<State>> GetAsync(int id)
        =>await _staterepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<State>>> GetAsync()
        => await _staterepository.GetAsync();
        public override async Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination)
        => await _staterepository.GetAsync(pagination);
        public async Task<IEnumerable<State>> GetComboAsync(int countryId)
        =>await _staterepository.GetComboAsync(countryId);
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        => await _staterepository.GetRecordsNumberAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _staterepository.GetTotalPagesAsync(pagination);
    }
}
