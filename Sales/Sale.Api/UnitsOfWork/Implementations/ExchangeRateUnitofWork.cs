using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;
using System.Diagnostics;

namespace Sale.Api.UnitsOfWork.Implementations
{
    public class ExchangeRateUnitofWork :GenericUnitOfWork<ExchangeRate> ,IexchangeRateUnitofWork
    {
        private readonly IexchangeRateRepository _exchangeRateRepository;

        public ExchangeRateUnitofWork(IGenericRepository<ExchangeRate> repository, IexchangeRateRepository exchangeRateRepository) : base(repository)
        {
           _exchangeRateRepository = exchangeRateRepository;
        }

        public async Task<ActionResponse<ExchangeRate>> AddFullAsync(ExchangeRateDTO exchangeRateDTO)
        => await _exchangeRateRepository.AddFullAsync(exchangeRateDTO);

        public override async Task<ActionResponse<ExchangeRate>> GetAsync(int id)
        => await _exchangeRateRepository.GetAsync(id);

        public override async Task<ActionResponse<IEnumerable<ExchangeRate>>> GetAsync(PaginationDTO pagination)
        => await _exchangeRateRepository.GetAsync(pagination);

        public async Task<IEnumerable<Currency>> GetComboAsync()
        => await _exchangeRateRepository.GetComboAsync();   

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        => await _exchangeRateRepository.GetRecordsNumberAsync(paginationDTO);

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        => await _exchangeRateRepository.GetTotalPagesAsync(paginationDTO);
    }
}
