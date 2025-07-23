using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class StatesRepository :GenericRepository<State>, IStatesRepository
    {
        private readonly DataContext _context;

        public StatesRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<State>> GetAsync(int id)
        {
            var state=await _context.states.Include(x=>x.cities).FirstOrDefaultAsync(x => x.Id == id);
            if (state == null)
            {
                return new ActionResponse<State>
                {
                    WasSuccess = false,
                    Message = "State does not exist"
                };
            }
            return new ActionResponse<State>
            {
                WasSuccess=true,
                Result= state
            };             

        }

        public override async Task<ActionResponse<IEnumerable<State>>> GetAsync()
        {
            var states=await _context.states.Include(x=>x.cities).ToListAsync();
            return new ActionResponse<IEnumerable<State>>
            {
                WasSuccess = true,
                Result = states
            };
        }

        public override async Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination)
        {
            var queryable=_context.states.Include(x=>x.Country).Include(x=>x.cities).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<State>>
            {
                WasSuccess = true,
                Result =await queryable.OrderBy(x => x.Name).ToListAsync()
            };
        }

        public async Task<IEnumerable<State>> GetComboAsync(int countryId)
        {
           return await _context.states.Where(x=>x.CountryId== countryId).OrderBy(x=>x.Name).ToListAsync();    
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.states.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var countAsync=await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = countAsync,
            };
        }
        
        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
           var queryable=_context.states.AsQueryable();
           if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double countAsync = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(countAsync / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }
    }
}
