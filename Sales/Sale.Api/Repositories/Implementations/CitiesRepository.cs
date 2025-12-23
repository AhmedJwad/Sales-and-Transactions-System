using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class CitiesRepository :GenericRepository<City>, ICitiesRepository
    {
        private readonly DataContext _context;

        public CitiesRepository(DataContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.cities.Include(x=>x.State).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<City>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync()
            };
        }

        public async Task<IEnumerable<City>> GetComboAsync(int stateId)
        {
            return await _context.cities.Where(x=>x.StateId == stateId).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.cities.AsQueryable();
            if (pagination.Id != 0)
            {
                queryable = queryable.Where(x => x.State!.Id == pagination.Id).AsQueryable();
            }

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
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
            var queryable = _context.cities.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages,
            };
        }
    }
}
