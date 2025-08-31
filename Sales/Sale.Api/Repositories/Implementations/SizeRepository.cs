using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class SizeRepository :GenericRepository<Sizep>, IsizeRepository
    {
        private readonly DataContext _context;

        public SizeRepository(DataContext context) : base(context)
        {
          _context = context;
        }

        public override async Task<ActionResponse<Sizep>> GetAsync(int id)
        {
            var size=await _context.sizes.Include(x=>x.productSizes!).ThenInclude(x=>x.product).FirstOrDefaultAsync(x=>x.Id==id);
            if(size==null)
            {
                return new ActionResponse<Sizep>
                {
                    WasSuccess = false,
                    Message = "size does not exist"
                };
            }
            return new ActionResponse<Sizep>
            {
                WasSuccess = true,
                Result = size,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Sizep>>> GetAsync()
        {
            var size = await _context.sizes.OrderBy(x => x.Name).ToListAsync();
            if (size == null) 
            {
                return new ActionResponse<IEnumerable<Sizep>>
                {
                    WasSuccess = false,
                    Message = "size does not exist"
                };
            }
            return new ActionResponse<IEnumerable<Sizep>>
            {
                WasSuccess = true,
                Result = size,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Sizep>>> GetAsync(PaginationDTO pagination)
        {
            var queryable=_context.sizes.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Sizep>>
            {
                WasSuccess = true,
                Result =await queryable.OrderBy(x=>x.Name).ToListAsync(),
            };
        }

        public async Task<IEnumerable<Sizep>> GetComboAsync()
        {
            return await _context.sizes.OrderBy(x => x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.sizes.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            var countasync = await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = countasync
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
           var queryable=_context.sizes.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable=queryable.Where(x=>x.Name.ToLower().Contains($"{pagination.Filter.ToLower()}"));
            }
            double countasync= await queryable.CountAsync();
            var totalPage = (int)Math.Ceiling(countasync / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPage,
            };
        }
    }
}
