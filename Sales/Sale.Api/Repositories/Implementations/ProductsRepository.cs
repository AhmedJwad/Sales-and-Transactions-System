using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Enums;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class ProductsRepository :GenericRepository<Product>, IProductsRepository
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;

        public ProductsRepository(DataContext context , IFileStorage fileStorage) : base(context)
        {
           _context = context;
           _fileStorage = fileStorage;
        }

        public async Task<ActionResponse<Product>> AddFullAsync(ProductDTO productDTO)
        {
            try
            {
                productDTO.Barcode = GenerateBarcodeValue();
                var Newproduct = new Product
                {
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Barcode = productDTO.Barcode,
                    Cost = productDTO.Cost,
                    HasSerial = true,
                    Price = productDTO.Price,
                    DesiredProfit = productDTO.DesiredProfit / 100,
                    Stock = productDTO.Stock,
                    ProductCategories = new List<ProductCategory>(),
                    ProductImages=new List<ProductImage>(),
                    serialNumbers=new List<SerialNumber>(),
                };
               if(!productDTO.HasSerial)
                { 
                    string newSerialNumber = GenerateSerialNumber(8);

                    while (await _context.serialNumbers.AnyAsync(x => x.SerialNumberValue == newSerialNumber))
                    {
                        newSerialNumber = GenerateSerialNumber(8);
                    }
                    Newproduct.serialNumbers.Add(new SerialNumber { SerialNumberValue = newSerialNumber , SerialStatus=SerialStatus.Available});
                    
                }
                foreach (var productimage in productDTO.ProductImages!)
                {
                    var photoProduct = Convert.FromBase64String(productimage);
                    Newproduct.ProductImages.Add(new ProductImage
                    {
                        Image = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products")
                    });
                }
                foreach (var category in productDTO.ProductCategoryIds!)
                {
                    var Productcategory = await _context.Categories.FirstOrDefaultAsync(x => x.Id == category);
                    if(Productcategory != null)
                    {
                        Newproduct.ProductCategories.Add(new ProductCategory { Category = Productcategory });
                    }
                }

               
                _context.Add(Newproduct);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                    Result = Newproduct
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "A product with the same name already exists."
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        private string GenerateSerialNumber(int length)
        {
            
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();            
            var serialNumber = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return serialNumber;
        }
        private string GenerateBarcodeValue()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();
        }
        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products.Include(x => x.ProductImages)
                 .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }
            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("images/products"))
                {
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".jpg", "products");
                    product.ProductImages!.Add(new ProductImage { Image = imageDTO.Images[i] });
                }

            }
            _context.Update(product);
            await _context.SaveChangesAsync();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO
            };
        }

        public async Task<ActionResponse<Product>> DeleteAsync(int id)
        {
            var product = await _context.Products
               .Include(x => x.ProductCategories)
               .Include(x => x.ProductImages)
               .Include(x=>x.serialNumbers)
               .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Product not found"
                };
            }

            foreach (var productImage in product.ProductImages!)
            {
                await _fileStorage.RemoveFileAsync(productImage.Image, "products");
            }

            try
            {
                _context.ProductCategories.RemoveRange(product.ProductCategories!);
                _context.ProductImages.RemoveRange(product.ProductImages!);
                _context.serialNumbers.RemoveRange(product.serialNumbers!);
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                };
            }
            catch
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "The product cannot be deleted because it has related records."
                };
            }
        }

        public override async Task<ActionResponse<Product>> GetAsync(int id)
        {
            var product = await _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.ProductCategories!)
                .ThenInclude(x => x.Category)
                .Include(x=>x.serialNumbers)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }

            return new ActionResponse<Product>
            {
                WasSuccess = true,
                Result = product
            };
        }

        public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products
               .Include(x => x.ProductImages)
               .Include(x => x.ProductCategories)
               .Include(x=>x.serialNumbers)
               .AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            if (pagination.CategoryId != null && pagination.CategoryId > 0)
            {
                queryable = queryable.Where(x => x.ProductCategories!
                    .Any(y => y.CategoryId == pagination.CategoryId));
            }

            if (!string.IsNullOrWhiteSpace(pagination.CategoryFilter))
            {
                queryable = queryable.Where(x => x.ProductCategories!.Any(y => y.Category!.Name == pagination.CategoryFilter));
            }

            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = await queryable
                    .OrderBy(x => x.Name)
                    .Paginate(pagination)
                    .ToListAsync()
            };
        }

        public async Task<IEnumerable<Product>> GetComboAsync()
        {
            return await _context.Products
             .OrderBy(x => x.Name)
             .ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO paginationDTO)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(paginationDTO.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO paginationDTO)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(paginationDTO.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(paginationDTO.Filter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(paginationDTO.CategoryFilter))
            {
                queryable = queryable.Where(x => x.ProductCategories!.Any(y => y.Category.Name == paginationDTO.CategoryFilter));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / paginationDTO.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products
                   .Include(x => x.ProductImages)
                   .FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }
            if (product.ProductImages is null || product.ProductImages.Count == 0)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = true,
                    Result = imageDTO
                };
            }
            var lastImage = product.ProductImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
            _context.ProductImages.Remove(lastImage);
            await _context.SaveChangesAsync();
            imageDTO.Images = product.ProductImages.Select(x => x.Image).ToList();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO
            };
        }

        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products.Include(x => x.ProductCategories!).
                    ThenInclude(x => x.Category).FirstOrDefaultAsync(x => x.Id == productDTO.Id);
                if (product == null)
                {
                    return new ActionResponse<Product>
                    {
                        WasSuccess = false,
                        Message = "Product does not exist",
                    };
                }
                product.Name = productDTO.Name;
                product.Description = productDTO.Description;
                product.Price = productDTO.Price;
                product.Stock = productDTO.Stock;
                product.Cost = productDTO.Cost;
                product.DesiredProfit = productDTO.DesiredProfit / 100;
                _context.ProductCategories.RemoveRange(product.ProductCategories!);
                product.ProductCategories = new List<ProductCategory>();

                foreach (var item in productDTO.ProductCategoryIds!)
                {
                    var category = await _context.Categories.FindAsync(item);
                    if (category != null)
                    {
                        _context.ProductCategories.Add(new ProductCategory { CategoryId = category.Id, ProductId = product.Id });
                    }
                }
                _context.Update(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                    Result = product
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "A product with the same name already exists."
                };
            }
            catch (Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
    }
}
