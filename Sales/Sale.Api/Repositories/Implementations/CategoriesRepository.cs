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
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            return new ActionResponse<IEnumerable<Category>>
            {
                WasSuccess = true,
                Result = await queryable.OrderBy(x => x.Name).ToListAsync()
            };
        }

        public async Task<IEnumerable<CategoryDTO>> GetComboAsync()
        {
            return await _context.Categories.OrderBy(x => x.Name).Select(x=> new CategoryDTO
            {
                Id = x.Id,
                Name = x.Name,
                Subcategories = x.subcategories!.OrderBy(s => s.Name).Select(s => s.Name).ToList(),
                photo=x.Photo,
            }).ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination?.Filter))
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
            var queryable = _context.Categories.AsQueryable();
            if (!string.IsNullOrWhiteSpace(pagination?.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            double count = await queryable.CountAsync();
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
                    Name = categoryDTO.Name,
                    subcategories = new List<Subcategory>(),
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
                var category = await _context.Categories.Include(x => x.subcategories).FirstOrDefaultAsync(x => x.Id == categoryDTO.Id);
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
                category.Name = categoryDTO.Name;
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
        public override async Task<ActionResponse<Category>> DeleteAsync(int Id)
        {
            try
            {
                var category = await _context.Categories.Include(x => x.subcategories).FirstOrDefaultAsync(x => x.Id == Id);
                if (category == null)
                {
                    return new ActionResponse<Category>
                    {
                        WasSuccess = false,
                        Message = "category does not exist"
                    };
                }
                await _fileStorage.RemoveFileAsync(category.Photo!, "categories");

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
