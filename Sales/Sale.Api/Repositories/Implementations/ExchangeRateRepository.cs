using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class ExchangeRateRepository :GenericRepository<ExchangeRate>,  IexchangeRateRepository
    {
        private readonly DataContext _context;

        public ExchangeRateRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ActionResponse<ExchangeRate>> AddFullAsync(ExchangeRateDTO exchangeRateDTO)
        {
            try
            {
                var exchangerate = await _context.exchangeRates.Where(er => er.BaseCurrencyId == exchangeRateDTO.BaseCurrencyId
                && er.TargetCurrencyId == exchangeRateDTO.TargetCurrencyId && er.IsActive).ToListAsync();
                if(exchangerate!=null && exchangerate.Count()>0)
                {
                    foreach (var item in exchangerate)
                    {
                        item.IsActive = false;
                        item.EndDate = DateTime.UtcNow;
                    }
                }
                var newExchangeRate = new ExchangeRate
                {
                    BaseCurrencyId = exchangeRateDTO.BaseCurrencyId,
                    TargetCurrencyId = exchangeRateDTO.TargetCurrencyId,
                    Rate = exchangeRateDTO.Rate,
                    IsActive = true,
                    StartDate = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                 _context.Add(newExchangeRate);
                await _context.SaveChangesAsync();
                return new ActionResponse<ExchangeRate>
                {
                    WasSuccess = true,
                    Result = newExchangeRate,
                };  

            }
            catch (Exception ex)
            {
                return new ActionResponse<ExchangeRate>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };  
            }
        }
       
        public override async Task<ActionResponse<ExchangeRate>> GetAsync(int id)
        {
            var exchangeRate = await _context.exchangeRates.Include(ct=> ct.TargetCurrency).Include(cb=>cb.BaseCurrency).FirstOrDefaultAsync(x => x.Id == id);
            if (exchangeRate == null)
            {
                return new ActionResponse<ExchangeRate>
                {
                    WasSuccess = false,
                    Message = "size does not exist"
                };
            }
            return new ActionResponse<ExchangeRate>
            {
                WasSuccess = true,
                Result = exchangeRate,
            };
        }

        public override async Task<ActionResponse<IEnumerable<ExchangeRate>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.exchangeRates.Include(ct=>ct.TargetCurrency).Include(cb=>cb.BaseCurrency).AsQueryable();
            // Apply filtering based on pagination DTO
            if (!string.IsNullOrEmpty(pagination.Filter))
            {
                queryable = queryable.Where(er =>
                    er.BaseCurrency.Code.Contains(pagination.Filter) ||
                    er.TargetCurrency.Code.Contains(pagination.Filter));
            }
            return new ActionResponse<IEnumerable<ExchangeRate>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(er => er.BaseCurrency.Code)
                    .ThenBy(er => er.TargetCurrency.Code)
                    .Paginate(pagination)
                    .ToListAsync()
            }; 
        }

        public async Task<IEnumerable<ExchangeRate>> GetComboAsync()
        {
            return await _context.exchangeRates.ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        {
            var queryable = _context.exchangeRates.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(er =>
                  er.BaseCurrency.Code.Contains(paginationDTO.Filter) ||
                  er.TargetCurrency.Code.Contains(paginationDTO.Filter));
            }
            var countasync = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = countasync
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        {
            var queryable = _context.exchangeRates.AsQueryable();
            if (!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(er =>
                  er.BaseCurrency.Code.Contains(paginationDTO.Filter) ||
                  er.TargetCurrency.Code.Contains(paginationDTO.Filter));
            }
            double countasync = await queryable.CountAsync();
            var totalPage = (int)Math.Ceiling(countasync / paginationDTO.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPage,
            };
        }       
    }
}
