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
       
        public override  async Task<ActionResponse<IEnumerable<Subcategory>>> GetAsync(PaginationDTO pagination)
        {
            var lang = pagination.Language?.ToLower() ?? "en";
            var queryable = _context.subcategories.Include(c=>c.Category).
                ThenInclude(ct=>ct.categoryTranslations!.Where(t=>t.Language.ToLower()==lang)).Include(st=>st.SubcategoryTranslations!
            .Where(t=>t.Language.ToLower()==lang)).AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.SubcategoryTranslations!.Any(t => t.Language.ToLower()==lang && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
            }
            var query = await queryable.OrderBy(s => s.SubcategoryTranslations!.FirstOrDefault(s => s.Language == lang)!.Name)
                .Paginate(pagination).ToListAsync();
          
            return new ActionResponse<IEnumerable<Subcategory>>
            {
                WasSuccess = true,
                Result = query
            };
        }

        public override async Task<ActionResponse<Subcategory>> GetAsync(int Id)
        {
            var subcategory = await _context.subcategories.Include(s => s.SubcategoryTranslations)
                .Include(s => s.Brands).AsNoTracking().FirstOrDefaultAsync(s => s.Id == Id);
            if (subcategory == null)
            {
                return new ActionResponse<Subcategory>
                {
                    WasSuccess = false,
                    Message = "subcategory does not exist"
                };
            }
            return new ActionResponse<Subcategory>
            {
                WasSuccess = true,
                Result = subcategory
            };
         }
        public async Task<IEnumerable<SubcategoryDTO>> GetComboAsync(string lang = "en")
        {
            lang = lang.ToLower();
            return await _context.subcategories.Select(s=> new SubcategoryDTO
            {
                Id = s.Id,
                SubcategoryTranslations = s.SubcategoryTranslations!.Where(t=>t.Language.ToLower()==lang)
                .Select(t=> new SubcategoryTranslationDto
                {                   
                    Name=t.Name,
                    Language=t.Language,
                }).ToList(),
            }).ToListAsync();
        }

        public async Task<IEnumerable<SubcategoryDTO>> GetComboAsync(int categoryId, string lang = "en")
        {
            lang = lang.ToLower();
            const string fallbackLang = "en";

            var query = await _context.subcategories
                .Where(s => s.CategoryId == categoryId)
                .Select(s => new SubcategoryDTO
                {
                    Id = s.Id,                  
                    SubcategoryTranslations = s.SubcategoryTranslations!
                        .Where(t => t.Language.ToLower() == lang)
                        .Select(t => new SubcategoryTranslationDto
                        {
                            Language = t.Language,
                            Name = t.Name
                        }).ToList()
                        ?? 
                            s.SubcategoryTranslations!
                                .Where(t => t.Language.ToLower() == fallbackLang)
                                .Select(t => new SubcategoryTranslationDto
                                {
                                    Language = t.Language,
                                    Name = t.Name
                                })
                                .ToList(),
                    Photo = s.Photo,
                    Products = s.Prosubcategories!
                        .Select(ps => ps.Product)                      
                        .Select(p => new ProductDTO
                        {
                            Id = p.Id,                            
                            Price = p.Price
                        })
                        .ToList()
                })
                .ToListAsync();

            return query;
        }


        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.subcategories.Include(st => st.SubcategoryTranslations).AsQueryable();
            if (pagination.Id !=0)
            {
                queryable = queryable.Where(x => x.Category!.Id == pagination.Id).AsQueryable();
            }
            var lang = pagination.Language?.ToLower() ?? "en";          
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.SubcategoryTranslations!.Any(t => t.Language.ToLower() == lang && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
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
            var queryable = _context.subcategories.Include(st => st.SubcategoryTranslations).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(s => s.SubcategoryTranslations!.Any(t => t.Language.ToLower() == pagination.Language!.ToLower()
                && t.Name.ToLower().Contains(pagination.Filter.ToLower())));
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
                    Category=await _context.Categories.FindAsync(subcategoryDTO.CategoryId),
                    SubcategoryTranslations=subcategoryDTO.SubcategoryTranslations!.Select(s=> new SubcategoryTranslation
                    {
                        Language=s.Language,
                        Name=s.Name,
                    }).ToList()
                };
                if (!string.IsNullOrEmpty(subcategoryDTO.Photo))
                {
                    var productPhoto = Convert.FromBase64String(subcategoryDTO!.Photo!);
                    subcategory.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "subcategories");
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
                var subCategory = await _context.subcategories.Include(S=>S.SubcategoryTranslations).FirstOrDefaultAsync(x => x.Id == subcategoryDTO.Id);
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
                    if (!subcategoryDTO.Photo.StartsWith("images/subcategories"))
                    {
                       var productPhoto = Convert.FromBase64String(subcategoryDTO!.Photo!);
                        subCategory.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "subcategories");
                    }else
                    {
                        subCategory.Photo = subcategoryDTO.Photo;
                    }                   
                }
                _context.subcategoryTranslations.RemoveRange(subCategory.SubcategoryTranslations!);
                subCategory.CategoryId= subcategoryDTO.CategoryId;              
                subCategory.SubcategoryTranslations= subcategoryDTO.SubcategoryTranslations!.Select(s => new SubcategoryTranslation
                {
                    Language = s.Language,
                    Name = s.Name,
                    SubcategoryId = subCategory.Id
                }).ToList();
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
                var subCategory = await _context.subcategories.Include(x=>x.SubcategoryTranslations)
                  .FirstOrDefaultAsync(x => x.Id == Id);
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
                if (subCategory.SubcategoryTranslations != null && subCategory.SubcategoryTranslations.Any())
                {
                    _context.subcategoryTranslations.RemoveRange(subCategory.SubcategoryTranslations);
                }
                _context.Remove(subCategory);
                await _context.SaveChangesAsync();
                return new ActionResponse<Subcategory>
                {
                  WasSuccess=true,
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
