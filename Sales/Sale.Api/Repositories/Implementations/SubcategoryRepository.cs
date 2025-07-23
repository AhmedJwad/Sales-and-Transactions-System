using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class SubcategoryRepository :GenericRepository<Subcategory>, ISubcategoryRepository
    {
        private readonly DataContext _context;

        public SubcategoryRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination)
        {
            var queryable=_context.subcategories.Include(x=>x.Category).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter!.ToLower()));
            }
            return new ActionResponse<IEnumerable<Subcategory>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).ToListAsync(),
            };
        }

        public async Task<IEnumerable<Subcategory>> GetComboAsync()
        {
            return await _context.subcategories.OrderBy(x=>x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.subcategories.AsQueryable();
            if(pagination.Id !=0)
            {
                queryable = queryable.Where(x => x.Category!.Id == pagination.Id).AsQueryable();
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            int recordnumber=await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordnumber
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
           var queryable=_context.subcategories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter!.ToLower()));
            }
            double count=await queryable.CountAsync();
            int totalPage = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPage
            };
        }
    }
}
