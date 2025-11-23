using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class ColourRepositorycs :GenericRepository<Colour>, IcolourRepository
    {
        private readonly DataContext _context;

        public ColourRepositorycs(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<Colour>> GetAsync(int id)
        {
           var colour=await _context.colors!.Include(x=>x.productColor!).ThenInclude(x=>x.Product).FirstOrDefaultAsync(x=>x.Id == id);
            if(colour==null)
            {
                return new ActionResponse<Colour>
                {
                    WasSuccess = false,
                    Message = "colour does not exist",
                };
            }
            return new ActionResponse<Colour>
            {
                WasSuccess = true,
                Result = colour,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Colour>>> GetAsync()
        {
            var colour = await _context.colors.OrderBy(x => x.Name).ToListAsync();
            return new ActionResponse<IEnumerable<Colour>>
            {
                WasSuccess = true,
                Result = colour,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Colour>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.colors!.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Colour>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync(),
            };
        }

        public async Task<IEnumerable<Colour>> GetComboAsync()
        {
            return await _context.colors.OrderBy(x=>x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.colors.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            int countasync =await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result= countasync,
            };
            
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable=_context.colors.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains($"{pagination.Filter}"));
            }
            double count=await queryable.CountAsync();
            int totlaPage = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result= totlaPage,

            };
        }
    }
}
