using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;
using static System.Net.Mime.MediaTypeNames;

namespace Sale.Api.Repositories.Implementations
{
    public class CategoriesRepository :GenericRepository<Category>, ICategoriesRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public CategoriesRepository(DataContext context , IFileStorage fileStorage) : base(context)
        {
           _context = context;
           _fileStorage = fileStorage;
        }      

        public override async Task<ActionResponse<IEnumerable<Category>>> GetAsync(PaginationDTO pagination)
        {
            var lang = pagination.Language?.ToLower() ?? "en";
            var queryable = _context.Categories
                            .Include(c => c.categoryTranslations!
                                .Where(t => t.Language.ToLower() == lang))
                            .AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                
                queryable = queryable?.Where(c =>c.categoryTranslations!.Any(t => t.Language.ToLower() == lang && t.Name.ToLower()
                .Contains(pagination.Filter.ToLower()!))); 
            }
            return new ActionResponse<IEnumerable<Category>>
            {
                WasSuccess = true,
                Result = await queryable!.OrderBy(c => c.categoryTranslations!.FirstOrDefault(t => t.Language == lang)!.Name).Paginate(pagination!).ToListAsync()
            };
        }

        public async Task<IEnumerable<CategoryDTO>> GetComboAsync(string lang = "en")
        {
            lang = lang.ToLower();
            return await _context.Categories.Include(s=>s.subcategories)
                .Include(c=>c.categoryTranslations).Select(x=> new CategoryDTO
            {
                Id = x.Id,
                Name = x.categoryTranslations!
                    .Where(t => t.Language == lang)
                    .Select(t => t.Name)
                    .FirstOrDefault()
                ?? x.categoryTranslations!
                    .Where(t => t.Language == "en")
                    .Select(t => t.Name)
                    .FirstOrDefault()
                ?? "Unnamed",
                Subcategories = x.subcategories!.OrderBy(s => s.SubcategoryTranslations!.FirstOrDefault()!.Name).Select(s => s.SubcategoryTranslations!.FirstOrDefault()!.Name).ToList(),
                photo=x.Photo,
            }).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var lang = pagination.Language?.ToLower() ?? "en";
            var queryable = _context.Categories?.Include(x => x.categoryTranslations).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                var filter = pagination?.Language?.ToLower();
                queryable = queryable?.Where(c => c.categoryTranslations!.Any(t => t.Language.ToLower() == lang && t.Name.ToLower()
                .Contains(filter!)));
            }
            int recordsNumber = await queryable!.CountAsync();
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber,
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var lang = pagination.Language?.ToLower() ?? "en";
            var queryable = _context.Categories?.Include(x => x.categoryTranslations).AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                var filter = pagination?.Language?.ToLower();
                queryable = queryable?.Where(c => c.categoryTranslations!.Any(t => t.Language.ToLower() == lang && t.Name.ToLower()
                .Contains(filter!)));
            }
            double count = await queryable!.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination!.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages,
            };
        }
        public async Task<ActionResponse<Category>> AddFullAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var category = new Category
                {
                    subcategories = new List<Subcategory>(),
                    categoryTranslations = categoryDTO!.categoryTranslations!.Select(t => new CategoryTranslation
                    {
                        Language = t.Language.ToLower(),
                        Name = t.Name,
                    }).ToList()

                };              

                 if (!string.IsNullOrEmpty(categoryDTO.photo))
                {
                    var productPhoto = Convert.FromBase64String(categoryDTO!.photo!);
                    category.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "categories");
                }
                _context.Add(category);
                await _context.SaveChangesAsync();
                return new ActionResponse<Category>
                {
                    WasSuccess = true,
                    Result = category
                };

            }
            catch (Exception ex)
            {
                return new ActionResponse<Category>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }
        }
        public async Task<ActionResponse<Category>> UpdateFullAsync(CategoryDTO categoryDTO)
        {
            try
            {
                var category = await _context.Categories.Include(x => x.subcategories)
                    .Include(c=>c.categoryTranslations).FirstOrDefaultAsync(x => x.Id == categoryDTO.Id);
                if (category == null)
                {
                    return new ActionResponse<Category>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
                if (!string.IsNullOrEmpty(categoryDTO.photo))
                {
                    var productPhoto = Convert.FromBase64String(categoryDTO!.photo!);
                    category.Photo = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "categories");
                }
                _context.categoryTranslations.RemoveRange(category.categoryTranslations!);

                category.categoryTranslations = categoryDTO.categoryTranslations!.Select(t => new CategoryTranslation
                {
                    Language = t.Language.ToLower(),
                    Name = t.Name,
                    CategoryId = category.Id
                }).ToList();
                _context.Update(category);
                await _context.SaveChangesAsync();
                return new ActionResponse<Category>
                {
                    WasSuccess = false,
                    Result = category,
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Category>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
            }

        }
        public override async Task<ActionResponse<Category>> GetAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.categoryTranslations)
                .Include(s => s.subcategories)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return new ActionResponse<Category>
                {
                    WasSuccess = false,
                    Message = "Category not found"
                };
            }
            return new ActionResponse<Category>
            {
                WasSuccess = true,
                Result = category
            };  
        }
        public override async Task<ActionResponse<Category>> DeleteAsync(int Id)
        {
            try
            {
                var category = await _context.Categories.Include(x => x.subcategories).Include(t=>t.categoryTranslations)
                    .FirstOrDefaultAsync(x => x.Id == Id);
                if (category == null)
                {
                    return new ActionResponse<Category>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
                if(category.Photo!=null)
                {
                    await _fileStorage.RemoveFileAsync(category.Photo!, "categories");
                }             

                _context.Remove(category);
                await _context.SaveChangesAsync();
                return new ActionResponse<Category>
                {
                    WasSuccess = true
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<Category>
                {
                    WasSuccess = false,
                    Message = ex.Message,
                };
               
            }
        }

       
    }
}
