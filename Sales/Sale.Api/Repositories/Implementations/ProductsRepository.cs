using Microsoft.EntityFrameworkCore;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Interfaces;
using Sale.Share.DTOs;
using Sale.Share.Entities;
using Sale.Share.Response;
using Sale.Share.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Net.Mime.MediaTypeNames;

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
                    Name = productDTO.Name,
                    Description = productDTO.Description,
                    Stock = productDTO.Stock,
                    Cost = productDTO.Cost,
                    Barcode = string.IsNullOrEmpty(productDTO.Barcode) ? GenerateBarcode() : productDTO.Barcode,
                    Price = productDTO.Price,
                    HasSerial = productDTO.HasSerial,
                    CreatedAt = DateTime.UtcNow,
                    productsubCategories = new List<ProductsubCategory>(),
                    productColor=new List<ProductColor>(),
                    ProductImages = new List<ProductImage>(),
                    serialNumbers = new List<SerialNumber>(),
                    productSize=new List<productSize>(),
                    BrandId=productDTO.BrandId,
                    brand=_context.brands.Find(productDTO.BrandId),

                };
                var colors = await _context.colors!.Where(c => productDTO.ProductColorIds!.Contains(c.Id)).ToListAsync();
                foreach (var color in colors)
                {
                    if (!product.productColor!.Any(pc => pc.ColorId == color.Id))
                    {
                        product.productColor!.Add(new ProductColor
                        {
                            ProductId = product.Id,
                            ColorId = color.Id
                        });
                    }
                }

                var sizes = await _context.sizes!.Where(c => productDTO.ProductSizeIds!.Contains(c.Id)).ToListAsync();
                foreach (var size in sizes)
                {
                    if (!product.productSize!.Any(pc => pc.SizeId == size.Id))
                    {
                        product.productSize!.Add(new productSize
                        {
                           productId = product.Id,
                            SizeId = size.Id
                        });
                    }
                }
                foreach (var image in productDTO.ProductImages!)
                {
                    var productPhoto = Convert.FromBase64String(image);
                    var newImage = await _fileStorage.SaveFileAsync(productPhoto, ".jpg", "products");
                    //product.ProductImages.Add(new ProductImage { Image = await _fileStorage.SaveFileAsync(productPhoto,".jpg" , "products") });

                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        Image = newImage,
                        productColorImages = colors.Select(c => new ProductColorImage
                        {
                            ColorId = c.Id,


                        }).ToList()
                    };
                    product.ProductImages!.Add(productImage);
                }
                var subCategories = await _context.subcategories!
                                        .Include(x => x.Brands)
                                        .Where(x => productDTO.ProductCategoryIds!.Contains(x.Id))
                                        .ToListAsync();
                foreach (var subcategory in subCategories!)
                {                  
                  
                   product.productsubCategories.Add(new ProductsubCategory { Category = subcategory });
                   
                }
                var brands = subCategories
                            .SelectMany(sc => sc.Brands!)
                            .Distinct() 
                            .ToList();
                if (brands.Any())
                {
                    product.BrandId = brands.First().Id; 
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
            var product = await _context.Products
                        .Include(p => p.ProductImages!)
                        .ThenInclude(pi => pi.productColorImages)
                        .Include(p => p.productColor)
                        .FirstOrDefaultAsync(p => p.Id == imageDTO.ProductId);
            if (product == null)
            {
                return new ActionResponse<ImageDTO>
                {
                    WasSuccess = false,
                    Message = "Product does not exist",
                };               
            }
            var colors= await _context.colors!.Where(c =>imageDTO.ProductColorIds!.Contains(c.Id)).ToListAsync();
            foreach (var color in colors)
            {
                if (!product.productColor!.Any(pc => pc.ColorId == color.Id))
                {
                    product.productColor!.Add(new ProductColor
                    {
                        ProductId = product.Id,
                        ColorId = color.Id
                    });
                }
            }

            for (int i = 0; i < imageDTO.Images.Count; i++)
            {
                if (!imageDTO.Images[i].StartsWith("images/products"))
                {
                    var photoProduct = Convert.FromBase64String(imageDTO.Images[i]);
                    imageDTO.Images[i] = await _fileStorage.SaveFileAsync(photoProduct, ".png", "products");
                    var productImage = new ProductImage
                    {
                        ProductId = product.Id,
                        Image = imageDTO.Images[i],
                        productColorImages = colors.Select(c => new ProductColorImage
                        {
                            ColorId = c.Id,
                        }).ToList()
                    };
                    product.ProductImages!.Add(productImage);
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
            var product = await _context.Products.Include(x => x.productsubCategories).Include(x => x.ProductImages).Include(c=>c.serialNumbers)
                .Include(x=>x.brand).FirstOrDefaultAsync(x => x.Id == id);
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
                _context.serialNumbers.RemoveRange(product.serialNumbers!);
                _context.Remove(product);
                await _context.SaveChangesAsync();
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
                .Include(x=>x.productColor!).ThenInclude(x=>x.color).Include(x=>x.productSize!).ThenInclude(x=>x.size)
                .Include(x=>x.brand).FirstOrDefaultAsync(x=>x.Id == id);
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
            var queryable = _context.Products.Include(x => x.productsubCategories!).ThenInclude(x=>x.Category).Include(x=>x.productColor!)
                .ThenInclude(x=>x.color).Include(x => x.ProductImages).Include(x => x.brand)
                .Include(x => x.serialNumbers).Include(x=>x.productSize!).ThenInclude(x=>x.size).AsQueryable();
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
                queryable = queryable.Where(x => x.productsubCategories!.Any(x => x.Category!.SubcategoryTranslations!.Any(c=>c.Name==pagination.CategoryFilter)));
            }
            var products = await queryable.OrderBy(x => x.Name).ToListAsync();
            
            return new ActionResponse<IEnumerable<Product>>
            {
                WasSuccess = true,
                Result = products.OrderByDescending(p=>p.CreatedAt),
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
                queryable = queryable.Where(x => x.productsubCategories!.Any(y => y.Category!.SubcategoryTranslations!.Any(s=>s.Name==pagination.CategoryFilter)));
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
            var product = await _context.Products.Include(x => x.ProductImages!).ThenInclude(x=>x.productColorImages)
                .Include(x=>x.productColor).FirstOrDefaultAsync(x => x.Id == imageDTO.ProductId);
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
            if(lastImage !=null)
            {
                await _fileStorage.RemoveFileAsync(lastImage!.Image, "products");
                if (lastImage.productColorImages != null && lastImage.productColorImages.Any())
                {
                    _context.productColorImages.RemoveRange(lastImage.productColorImages);
                }              

            }         
           
            _context.ProductImages.Remove(lastImage!);
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
                    .Include(x => x.brand).Include(x => x.serialNumbers).Include(x=>x.productColor).Include(x=>x.productSize)
                    .FirstOrDefaultAsync(x => x.Id == productDTO.Id);
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
                _context.productColors.RemoveRange(product.productColor!);
                product.productColor = new List<ProductColor>();
                _context.productSizes.RemoveRange(product.productSize!);
                product.productSize = new List<productSize>();
                var subcategories = await _context.subcategories
                                  .Include(x => x.Brands)
                                  .Where(x => productDTO.ProductCategoryIds!.Contains(x.Id))
                                  .ToListAsync();
                product.BrandId = productDTO.BrandId;
                foreach (var item in subcategories!)
                {                   
                    _context.productsubCategories.Add(new ProductsubCategory { subcategoryId = item!.Id, ProductId = product.Id });
                    
                }
                var colors = await _context.colors!.Where(c => productDTO.ProductColorIds!.Contains(c.Id)).ToListAsync();
                foreach (var color in colors)
                {
                    if (!product.productColor!.Any(pc => pc.ColorId == color.Id))
                    {
                        _context.productColors!.Add(new ProductColor
                        {
                            ProductId = product.Id,
                            ColorId = color.Id
                        });
                    }
                }
                var sizes = await _context.sizes!.Where(c => productDTO.ProductSizeIds!.Contains(c.Id)).ToListAsync();
                foreach (var size in sizes)
                {
                    if (!product.productSize!.Any(pc => pc.SizeId == size.Id))
                    {
                        _context.productSizes!.Add(new productSize
                        {
                            productId = product.Id,
                            SizeId = size.Id
                        });
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
            catch(Exception exception)
            {
                return new ActionResponse<Product>
                {
                    WasSuccess = false,
                    Message = exception.Message
                };
            }
        }
        public async Task<IEnumerable<CategoryProductDTO>> GetProductCountByCategoryAsync(string lang = "en")
        {
            lang = lang.ToLower();

            var response = await (
                from prodCat in _context.productsubCategories
                join prod in _context.Products on prodCat.ProductId equals prod.Id
                join cat in _context.subcategories on prodCat.subcategoryId equals cat.Id
                from trans in cat.SubcategoryTranslations!
                    .Where(t => t.Language.ToLower() == lang)
                    .DefaultIfEmpty()
                group prodCat by new
                {
                    CategoryId = cat.Id,
                    CategoryName = trans != null ? trans.Name : "Unnamed"
                } into g
                select new CategoryProductDTO
                {
                    CategoryName = g.Key.CategoryName,
                    ProductCount = g.Count()
                }
            ).ToListAsync();

            return response;
        }


        public async Task<ActionResponse<IEnumerable<ProductDTO>>> GetProductsBySubcategoryAsync(int subcategoryId)
        {
            var products = await _context.Products.Where(x => x.productsubCategories!.Any(psc => psc.subcategoryId == subcategoryId))
               .Select(p => new ProductDTO
               {                 
                   Id = p.Id,
                   Name = p.Name,
                   Barcode = p.Barcode,
                   Description = p.Description,
                   Price = p.Price,
                   Cost = p.Cost,
                   DesiredProfit = p.DesiredProfit,
                   Stock = p.Stock,
                   BrandId = p.BrandId,
                   HasSerial = p.HasSerial,
                   ProductCategoryIds = p.productsubCategories!.Select(psc => psc.subcategoryId).ToList(),
                   ProductImages = p.ProductImages!.Select(img => img.Image).ToList(),
                   SerialNumbers = p.serialNumbers!.Select(sn => sn.SerialNumberValue).ToList()

               }).ToListAsync();
            return new ActionResponse<IEnumerable<ProductDTO>>
            {
                WasSuccess=true,
                Result=products,
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductResponseDTO>>> FilterProducts(ProductFilterDto productFilterDto)
        {
            var queryable = _context.Products.Include(pc => pc.productColor!).ThenInclude(c => c.color).Include(ps => ps.productSize!)
                 .ThenInclude(s => s.size).Include(b => b.brand).Include(pi => pi.ProductImages)
                 .Include(sn => sn.serialNumbers).Include(x=>x.productsubCategories!).ThenInclude(x=>x.Category).AsQueryable();           

            if (productFilterDto.BrandId.HasValue)
            {
               var temp = queryable.Where(p => p.BrandId == productFilterDto.BrandId);
                if (await temp.AnyAsync())
                {
                    queryable = temp;
                }
            }           

            if (productFilterDto.ColorIds != null && productFilterDto.ColorIds.Any())
            {
                var temp = queryable.Where(p => p.productColor!.Any(pc => productFilterDto.ColorIds.Contains(pc.ColorId)));
                if (await temp.AnyAsync())
                {
                    queryable = temp;
                }
            }
            if (productFilterDto.SizeIds != null && productFilterDto.SizeIds.Any())
            {
                var temp = queryable.Where(p => p.productSize!.Any(ps => productFilterDto.SizeIds.Contains(ps.SizeId)));
                if (await temp.AnyAsync())
                {
                    queryable = temp;
                }
            }
            if (productFilterDto.MaxPrice.HasValue)
            {
               var temp = queryable.Where(p => p.Price <= productFilterDto.MaxPrice);
                if (await temp.AnyAsync())
                {
                    queryable = temp;
                }
            }
            if (productFilterDto.MinPrice.HasValue)
            {
                var temp = queryable.Where(p => p.Price >= productFilterDto.MinPrice);
                if (await temp.AnyAsync())
                {
                    queryable = temp;
                }
            }
            var products = await queryable.ToListAsync();
            if (!products.Any())
            {
                return new ActionResponse<IEnumerable<ProductResponseDTO>>
                {
                    WasSuccess = false,
                    Message = "No products found with the given filters"
                };
            }
            var productresponse = products.Select(p => new ProductResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DesiredProfit = p.DesiredProfit,
                Barcode = p.Barcode,
                brand = tobrand(p.brand),
                Price = p.Price,
                BrandId = p.BrandId,
                Cost = p.Cost,
                HasSerial = p.HasSerial,
                ProductColor = p.productColor!.Select(pc => pc.color!.Name).ToList(),
                ProductSize = p.productSize!.Select(ps => ps.size!.Name).ToList(),
                ProductImages = p.ProductImages!.Select(pi => pi.Image).ToList(),              
                ProductSubCategories = p.productsubCategories!
                                        .SelectMany(ps => ps.Category!.SubcategoryTranslations!                                           
                                        .Select(t => t.Name))
                                        .ToList(),
                SerialNumbers = p.serialNumbers!.Select(sn => sn.SerialNumberValue).ToList(),
                Stock = p.Stock,
            });
            return new ActionResponse<IEnumerable<ProductResponseDTO>>
            {
                WasSuccess = true,
                Result = productresponse,
            };

        }

        private BrandDTO tobrand(Brand? brand)
        {
            if (brand == null)
            {
                return new BrandDTO();
            }
                
            return new BrandDTO
            {
                Id = brand!.Id,
                //Name = brand.BrandTranslations!.FirstOrDefault(t=>t.Language=="en")!.Name
            };
        }

        public async Task<ActionResponse<IEnumerable<ProductResponseDTO>>> GetfullProduct()
        {
            var product =await _context.Products.Select(p => new ProductResponseDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                DesiredProfit = p.DesiredProfit,
                Barcode = p.Barcode,
                Cost = p.Cost,
                Stock = p.Stock,
                HasSerial = p.HasSerial,
                Price = p.Price,
                CreatedAt=p.CreatedAt,
                brand = p.brand != null ? new BrandDTO
                {
                    Id=p.brand.Id,
                    //Name=p.brand.BrandTranslations!.FirstOrDefault(t=>t.Language=="en")!.Name,
                } : null,
                ProductSubCategories=p.productsubCategories!.Where
                (sc=>sc.Category != null).SelectMany(sc=>sc.Category!.SubcategoryTranslations!).Select(s=>s.Name).ToList(),
                ProductImages=p.ProductImages!.Select(pi=>pi.Image).ToList(),
                ProductColor=p.productColor!.Where(pc=>pc.color!=null).Select(pc=>pc.color!.HexCode).ToList(),
                ProductSize=p.productSize!.Where(ps=>ps.size !=null).Select(ps=>ps.size!.Name).ToList(),
            }).ToListAsync();
            return new ActionResponse<IEnumerable<ProductResponseDTO>>
            {
                WasSuccess = true,
                Result = product.OrderByDescending(p=>p.CreatedAt),
            };
        }
        public async Task<ActionResponse<List<Product>>> GetProductsByIdsAsync(List<int> ids)
        {
            try
            {
                var products = await _context.Products
                    .Where(p => ids.Contains(p.Id)).Include(pi=>pi.ProductImages)                    
                    .ToListAsync();

                return new ActionResponse<List<Product>>
                {
                    WasSuccess = true,
                    Result = products
                };
            }
            catch (Exception ex)
            {
                return new ActionResponse<List<Product>>
                {
                    WasSuccess = false,
                    Message = ex.Message
                };
            }
        }

    }
}
