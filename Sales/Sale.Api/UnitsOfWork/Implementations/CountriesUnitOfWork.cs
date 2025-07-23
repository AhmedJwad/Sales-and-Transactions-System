using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class CountriesUnitOfWork :GenericUnitOfWork<Country> ,ICountriesUnitOfWork
    {
        private readonly ICountriesRepository _countriesRepository;

        public CountriesUnitOfWork(IGenericRepository<Country> repository, ICountriesRepository countriesRepository) : base(repository)
        {
           _countriesRepository = countriesRepository;
        }

        public override async Task<ActionResponse<Country>> GetAsync(int id)
        => await _countriesRepository.GetAsync(id);
        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync()
        => await _countriesRepository.GetAsync();
        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination)
        => await _countriesRepository.GetAsync(pagination);
        public async Task<IEnumerable<Country>> GetComboAsync()
        => await _countriesRepository.GetComboAsync();
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        =>await _countriesRepository.GetRecordsNumberAsync(pagination);
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        => await _countriesRepository.GetTotalPagesAsync(pagination);
    }
}
