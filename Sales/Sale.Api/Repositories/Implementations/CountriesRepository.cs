using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class CountriesRepository :GenericRepository<Country>, ICountriesRepository
    {
        private readonly DataContext _context;

        public CountriesRepository(DataContext context) : base(context)
        {
           _context = context;
        }
        public override async Task<ActionResponse<Country>> GetAsync(int id)
        {
            var country=await _context.countries!.Include(x=>x.states!).ThenInclude(x=>x.cities)
                .FirstOrDefaultAsync(x=>x.Id==id);
            if(country == null )
            {
                return new ActionResponse<Country>
                {
                    WasSuccess = false,
                    Message = "country does not exist",
                };
            }
            return new ActionResponse<Country>
            {
                WasSuccess = true,
                Result = country
            };
        }

        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync()
        {
            var countries = await _context.countries.OrderBy(x => x.Name).ToArrayAsync();
            return new ActionResponse<IEnumerable<Country>>
            {
                WasSuccess = true,
                Result = countries
            };
        }

        public override async Task<ActionResponse<IEnumerable<Country>>> GetAsync(PaginationDTO pagination)
        {
            var queryable=_context.countries.Include(x=>x.states!).ThenInclude(x=>x.cities).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Country>>
            {
                WasSuccess = true,
                Result =await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<IEnumerable<Country>> GetComboAsync()
        {
            return await _context.countries.OrderBy(x=>x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.countries.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber,
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable= _context.countries.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count=await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages,
            };
        }
    }
}
