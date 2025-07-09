using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Responses;

namespace Sale.Api.Repositories.Implementations
{
    public class ProductsRepository :GenericRepository<Product>, IProductRepository
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
                var product = new Product
                {
                   Name=productDTO.Name,
                   Description=productDTO.Description,
                   Stock=productDTO.Stock,
                   Cost=productDTO.Cost,
                   Barcode= string.IsNullOrEmpty(productDTO.Barcode) ? GenerateBarcode() : productDTO.Barcode,
                   Price =productDTO.Price,
                   BrandId=productDTO.BrandId,
                   HasSerial=productDTO.HasSerial,
                   productsubCategories=new List<ProductsubCategory>(),
                   ProductImages=new List<ProductImage>(),
                   serialNumbers=new List<SerialNumber>(),                 

                };
                foreach (var image in productDTO.ProductImages!)
                {
                    var productPhoto = Convert.FromBase64String(image);
                    product.ProductImages.Add(new ProductImage { Image = await _fileStorage.SaveFileAsync(productPhoto,".png" , "prodcuts") });
                }
                var subCategories = await _context.subcategories!
                                        .Include(x => x.Brands)
                                        .Where(x => productDTO.ProductCategoryIds!.Contains(x.Id))
                                        .ToListAsync();
                foreach (var subcategory in subCategories!)
                {                  
                  
                   product.productsubCategories.Add(new ProductsubCategory { Category = subcategory });
                    
                }

                if (product.HasSerial && product.Stock > 0)
                {
                    for (int i = 0; i < product.Stock; i++)
                    {
                        product.serialNumbers.Add(new SerialNumber
                        {
                            Product = product,
                            PurchaseDate = DateTime.Now,
                            SerialNumberValue = GenerateBarcode(12), 
                            SerialStatus = Share.Enums.SerialStatus.Used
                        });
                    }
                }
                else
                {                    
                    product.serialNumbers.Add(new SerialNumber
                    {
                        Product = product,
                        PurchaseDate = DateTime.Now,
                        SerialNumberValue = product.Barcode,
                        SerialStatus = Share.Enums.SerialStatus.Used
                    });
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return new ActionResponse<Product>
                {
                    WasSuccess = true,
                    Result = product,
                };
            }
            catch (DbUpdateException)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "A product with the same name already exists."
                };
            }catch(Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }

        private string GenerateBarcode(int length = 10)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public async Task<ActionResponse<ImageDTO>> AddImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if(product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist",
                };               
            }
            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("images/products"))
                {
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".png", "products");
                    product.ProductImages!.Add(new ProductImage { Image=imageDTO.Images[i] });
                }
            }
            _context.Update(product);
            await _context.SaveChangesAsync();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO,
            };
        }

        public override async Task<ActionResponse<Product>> DeleteAsync(int id)
        {
            var product = await _context.Products.Include(x => x.productsubCategories).Include(x => x.ProductImages)
                 .FirstOrDefaultAsync(x => x.Id == id);
            if(product == null)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = "Product not found"
                };
            }
            foreach (var productimage in product.ProductImages!)
            {
                await _fileStorage.RemoveFileAsync(productimage.Image, "products");
            }
            try
            {
                _context.productsubCategories.RemoveRange(product.productsubCategories!);
                _context.ProductImages.RemoveRange(product.ProductImages);
                _context.brands.RemoveRange(product.brand!);
                _context.Remove(product);
                return new ActionResponse<Product>
                {
                    WasSuccess=true,
                };
            }
            catch (Exception)
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
            var product=await _context.Products!.Include(x=>x.ProductImages!)
                .Include(x=>x.productsubCategories!).ThenInclude(x=>x.Category).ThenInclude(x=>x.Category).Include(x=>x.serialNumbers)
                .FirstOrDefaultAsync(x=>x.Id == id);
            if(product == null)
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
                Result = product,
            };
        }

        public override async Task<ActionResponse<IEnumerable<Product>>> GetAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.Include(x => x.productsubCategories!).ThenInclude(x => x.Category).ThenInclude(x => x.Category)
                .Include(x => x.ProductImages).Include(x => x.brand).Include(x => x.serialNumbers).AsQueryable();
            if(!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }
            if(pagination.CategoryId != null && pagination.CategoryId > 0)
            {
                queryable=queryable.Where(x=>x.productsubCategories!.Any(x=>x.subcategoryId==pagination.CategoryId));
            }
            if(!string.IsNullOrWhiteSpace(pagination.CategoryFilter))
            {
                queryable = queryable.Where(x => x.productsubCategories!.Any(x => x.Category.Name == pagination.CategoryFilter));
            }
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result =await queryable.OrderBy(x => x.Name).Paginate(pagination).ToListAsync()
            };

        }

        public async Task<IEnumerable<Product>> GetComboAsync()
        {
            return await _context.Products
             .OrderBy(x => x.Name)
             .ToListAsync();
        }

        public override async Task<ActionResponse<int>> GetRecordsNumberAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            int recordsNumber = await queryable.CountAsync();

            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = recordsNumber
            };
        }

        public override async Task<ActionResponse<int>> GetTotalPagesAsync(PaginationDTO pagination)
        {
            var queryable = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(pagination.Filter))
            {
                queryable = queryable.Where(x => x.Name.ToLower().Contains(pagination.Filter.ToLower()));
            }

            if (!string.IsNullOrWhiteSpace(pagination.CategoryFilter))
            {
                queryable = queryable.Where(x => x.productsubCategories!.Any(y => y.Category!.Name == pagination.CategoryFilter));
            }

            double count = await queryable.CountAsync();
            int totalPages = (int)Math.Ceiling(count / pagination.RecordsNumber);
            return new ActionResponse<int>
            {
                WasSuccess = true,
                Result = totalPages
            };
        }

        public async Task<ActionResponse<ImageDTO>> RemoveLastImageAsync(ImageDTO imageDTO)
        {
            var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
            if(product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist"
                };
            }
            if(product.ProductImages is null || product.ProductImages.Count == 0)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = true,
                   Result=imageDTO,
                };
            }
            var lastImage=product.ProductImages.LastOrDefault();
            await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
            _context.ProductImages.Remove(lastImage);
            await _context.SaveChangesAsync();
            imageDTO.Images=product.ProductImages.Select(x=>x.Image).ToList();
            return new ActionResponse<ImageDTO>
            {
                WasSuccess = true,
                Result = imageDTO,
            };
        }

        public async Task<ActionResponse<Product>> UpdateFullAsync(ProductDTO productDTO)
        {
            try
            {
                var product = await _context.Products.Include(x => x.productsubCategories!).ThenInclude(x => x.Category).ThenInclude(x => x.Category)
                    .Include(x => x.brand).Include(x => x.serialNumbers).FirstOrDefaultAsync(x => x.Id == productDTO.Id);
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
                product.Barcode=productDTO.Barcode;               
                _context.productsubCategories.RemoveRange(product.productsubCategories!);
                product.productsubCategories = new List<ProductsubCategory>();

                var subcategories = await _context.subcategories
                                  .Include(x => x.Brands)
                                  .Where(x => productDTO.ProductCategoryIds!.Contains(x.Id))
                                  .ToListAsync();
                foreach (var item in subcategories!)
                {                   
                    _context.productsubCategories.Add(new ProductsubCategory { subcategoryId = item!.Id, ProductId = product.Id });
                    
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
            catch(Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
        public async Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync()
        {
            // Query to join Product, ProductCategory, and Category tables
            var response = await (from prodCat in _context.productsubCategories
                                  join prod in _context.Products on prodCat.ProductId equals prod.Id
                                  join cat in _context.subcategories on prodCat.subcategoryId equals cat.Id
                                  group prodCat by new { cat.Id, cat.Name } into g
                                  select new CategoryProductDTO
                                  {
                                      CategoryName = g.Key.Name,     // The name of the category
                                      ProductCount = g.Count()       // The number of products in that category
                                  }).ToListAsync();

            return response;
        }
    }
}
