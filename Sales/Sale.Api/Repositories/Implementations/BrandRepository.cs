using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;
using System.Collections.Immutable;

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
            var lang=pagination.Language!.ToLower()?? "en";
            var queryable = _context.brands.Include(x => x.Subcategory).ThenInclude(st=>st.SubcategoryTranslations!
                             .Where(t=>t.Language.ToLower()==lang)).Include(b => b.BrandTranslations!.Where(t => t.Language.ToLower() == lang)).AsNoTracking().AsQueryable();
            //Select(b => new Brand
            //{
            //    Id = b.Id,
            //    SubcategoryId = b.SubcategoryId,
            //    Subcategory = b.Subcategory,
            //    BrandTranslations = b.BrandTranslations!.Where(t => t.Language.ToLower() == lang).ToList()
            //}).AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.BrandTranslations!
                .Any(t => t.Language.ToLower() == lang && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
            }
            return new ActionResponse<IEnumerable<Brand>>
            {
                WasSuccess = true,
                Result = await queryable.Paginate(pagination).ToListAsync()
            };

        }

        public async Task<IEnumerable<Brand>> GetComboAsync(int subcategoryId , string lang = "en")
        {
            lang = lang.ToLower();
            var brands = await _context.brands
                .Where(b => b.SubcategoryId == subcategoryId)
                .Include(b => b.Subcategory)               
                .Include(b => b.BrandTranslations!.Where(t => t.Language == lang))
                .AsNoTracking()
                .ToListAsync();
            return brands;
        }

        public async Task<IEnumerable<Brand>> GetComboAsync(string lang = "en")
        {
            lang = lang.ToLower();

            var brands = await _context.brands
                .Include(b => b.Subcategory)
                .Include(b => b.BrandTranslations!.Where(t => t.Language == lang)) 
                .ToListAsync();

            return brands;
        }

       
        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable=_context.brands.Include(t=>t.BrandTranslations).AsQueryable();
            if(pagination.Id!=0)
            {
                queryable=queryable.Where(x=>x.Subcategory!.Id==pagination.Id).AsQueryable();
            }
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.BrandTranslations!
               .Any(t => t.Language.ToLower() == pagination.Language!.ToLower() && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
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
            var queryable = _context.brands.Include(b=>b.BrandTranslations).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.BrandTranslations!
                .Any(t => t.Language.ToLower() == pagination.Language!.ToLower() && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
            }
           double count = await queryable.CountAsync();
            int totlaPage = (int)Math.Ceiling(count / pagination.RecordsNumber);

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totlaPage,
            };

        }
        public override async Task<ActionResponse<Brand>> GetAsync(int Id)
        {
            var brand = await _context.brands.Include(s => s.BrandTranslations)
                .Include(s => s.Products).AsNoTracking().FirstOrDefaultAsync(s => s.Id == Id);
            if (brand == null)
            {
                return new ActionResponse<Brand>
                {
                    WasSuccess = false,
                    Message = "subcategory does not exist"
                };
            }
            return new ActionResponse<Brand>
            {
                WasSuccess = true,
                Result = brand
            };
        }
        public async Task<ActionResponse<Brand>> AddFullAsync(BrandDTO brandDTO)
        {
            try
            {
                var brand = new Brand
                {
                    SubcategoryId = brandDTO.SubcategoryId!,
                    BrandTranslations = brandDTO.brandTranslations!.GroupBy(bt=>bt.Language)
                    .Select(g=>g.First()).Select(bt => new BrandTranslation
                    {
                        Language = bt.Language,
                        Name = bt.Name,
                    }).ToList(),
                };
                _context.brands.Add(brand);
                await _context.SaveChangesAsync();
                return new ActionResponse<Brand>
                {
                    WasSuccess = true,
                    Result = brand,
                };
            }
            catch (Exception ex)
            {

                return new ActionResponse<Brand>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }
        }

        public async Task<ActionResponse<Brand>> UpdateFullAsync(BrandDTO brandDTO)
        {
            try
            {
                var brand = await _context.brands.Include(bt => bt.BrandTranslations).FirstOrDefaultAsync(b => b.Id == brandDTO.Id);
                if(brand == null)
                {
                    return new ActionResponse<Brand>
                    {
                        WasSuccess = false,
                        Message = "Brand not found.",
                    };
                }
                _context.brandTranslations.RemoveRange(brand.BrandTranslations!);
                brand.SubcategoryId = brandDTO.SubcategoryId;
                brand.BrandTranslations = brandDTO.brandTranslations!
                                    .GroupBy(bt => bt.Language)
                                     .Select(g => g.First())
                                     .Select(bt => new BrandTranslation
                                     {
                                         Language = bt.Language,
                                         Name = bt.Name
                                     })
                                     .ToList();
                _context.brands.Update(brand);
                await _context.SaveChangesAsync();
                return new ActionResponse<Brand>
                {
                    WasSuccess = true,
                    Result = brand,
                };

            }
            catch (Exception ex)
            {
                return new ActionResponse<Brand>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };

            }
        }
        public override async Task<ActionResponse<Brand>> DeleteAsync(int Id)
        {
            try
            {
                var brand = await _context.brands.Include(x => x.BrandTranslations).Include(x => x.Products)
                  .FirstOrDefaultAsync(x => x.Id == Id);
                if (brand == null)
                {
                    return new ActionResponse<Brand>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
               
                if (brand.BrandTranslations != null && brand.BrandTranslations.Any())
                {
                    _context.brandTranslations.RemoveRange(brand.BrandTranslations);
                }
                if (brand.Products != null && brand.Products.Any())
                {
                    _context.Products.RemoveRange(brand.Products);
                }

                _context.Remove(brand);
                await _context.SaveChangesAsync();
                return new ActionResponse<Brand>
                {
                    WasSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Brand>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };

            }
        }
    }
}
