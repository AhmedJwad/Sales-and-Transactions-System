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
        private readonly IFileStorage _fileStorage;

        public SubcategoryRepository(DataContext context, IFileStorage fileStorage) : base(context)
        {
           _context = context;
           _fileStorage = fileStorage;
        }

        public override async Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.subcategories.Include(x => x.Category).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
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

        public async Task<IEnumerable<SubcategoryDTO>> GetComboAsync(int categoryId)
        {
            return await (_context.subcategories.Include(x=>x.Prosubcategories!).ThenInclude(x=>x.Product).Where(x => x.CategoryId == categoryId).OrderBy(x => x.Name)
                .Select(x => new SubcategoryDTO 
            {
                Id = x.Id,
                Name = x.Name,
                Photo=x.Photo,
                Products=x.Prosubcategories!.Select(x=>x.Product).OrderBy(x=>x.Name).Select(p=> new ProductDTO
                {
                    Id=p.Id,
                    Name=p.Name,
                    Description=p.Description,
                    Price=p.Price,
                }).ToList(),
            })
                .ToListAsync());
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
        public async Task<ActionResponse<Subcategory>> AddFullAsync(SubcategoryDTO subcategoryDTO)
        {
            try
            {
                var subcategory = new Subcategory
                {
                    Name = subcategoryDTO.Name,  
                    Category=await _context.Categories.FindAsync(subcategoryDTO.CategoryId),
                };
                if (!string.IsNullOrEmpty(subcategoryDTO.Photo))
                {
                    var productPhoto = Convert.FromBase64String(subcategoryDTO!.Photo!);
                    subcategory.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "categories");
                }
                _context.Add(subcategory);
                await _context.SaveChangesAsync();
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = true,
                    Result = subcategory
                };

            }
            catch (Exception ex)
            {
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ActionResponse<Subcategory>> UpdateFullAsync(SubcategoryDTO subcategoryDTO)
        {
            try
            {
                var subCategory = await _context.subcategories.FirstOrDefaultAsync(x => x.Id == subcategoryDTO.Id);
                if (subCategory == null)
                {
                    return new ActionResponse<Subcategory>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
                if (!string.IsNullOrEmpty(subcategoryDTO.Photo))
                {
                    var productPhoto = Convert.FromBase64String(subcategoryDTO!.Photo!);
                    subCategory.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "subcategories");
                }
                subCategory.Name = subcategoryDTO.Name;
                subCategory.CategoryId= subcategoryDTO.CategoryId;
                _context.Update(subCategory);
                await _context.SaveChangesAsync();
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = false,
                    Result = subCategory,
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }

        }
        public override async Task<ActionResponse<Subcategory>> DeleteAsync(int Id)
        {
            try
            {
                var subCategory = await _context.subcategories.FirstOrDefaultAsync(x => x.Id == Id);
                if (subCategory == null)
                {
                    return new ActionResponse<Subcategory>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
                if(!string.IsNullOrEmpty(subCategory.Photo))
                {
                    await _fileStorage.RemoveFileAsync(subCategory.Photo!, "subcategories");

                }           

                _context.Remove(subCategory);
                await _context.SaveChangesAsync();
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };

            }
        }
    }
}
