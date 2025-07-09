using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class BrandRepository : GenericRepository<Brand>, IbrandRepository
    {
        private readonly DataContext _context;

        public BrandRepository(DataContext context) : base(context)
        {
           _context = context;
        }

        public override async Task<ActionResponse<IEnumerable<Brand>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.brands.Include(x=>x.Subcategory).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
               queryable=queryable.Where(x=>x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Brand>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).ToListAsync()
            };

        }

        public async Task<IEnumerable<Brand>> GetComboAsync(int subcategoryId)
        {
            return await _context.brands.Where(x=>x.SubcategoryId==subcategoryId).OrderBy(x=>x.Name).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable=_context.brands.AsQueryable();
            if(pagination.Id!=0)
            {
                queryable=queryable.Where(x=>x.Subcategory!.Id==pagination.Id).AsQueryable();
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            int recordNumber=await queryable.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess=true,
                Result=recordNumber,
            };

        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.brands.AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
              queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
           double count = await queryable.CountAsync();
            int totlaPage = (int)Math.Ceiling(count / pagination.RecordsNumber);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totlaPage,
            };

        }
    }
}
