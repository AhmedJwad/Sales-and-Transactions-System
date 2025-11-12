using Microsoft.EntityFrameworkCore;
using Sale.Api.Helpers;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.Entities;
using Sale.Share.Enums;
using System.Runtime.InteropServices;
using static Org.BouncyCastle.Asn1.Cmp.Challenge;
using static System.Net.Mime.MediaTypeNames;

namespace Sale.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly IRuntimeInformationWrapper _runtimeInformationWrapper;
        private readonly IUsersUnitOfWork _usersUnitOfWork;

        public SeedDb(DataContext context, IFileStorage fileStorage, IRuntimeInformationWrapper runtimeInformationWrapper, 
            IUsersUnitOfWork usersUnitOfWork )
        {
            _context = context;
            _fileStorage = fileStorage;
            _runtimeInformationWrapper = runtimeInformationWrapper;
           _usersUnitOfWork = usersUnitOfWork;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();        
            await CheckCategoriesAsync();
            await CheckProductsAsync();
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUsersAsync();
            await CheckBrandAsunc();
        }

      

        private async Task CheckRolesAsync()
        {
            await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
            await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task CheckUsersAsync()
        {
            await CheckUserAsync("Ahmed", "Almershady", "Ahmed@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "AhmedAlmershady.jpg", UserType.Admin);
            await CheckUserAsync("Ledys", "Bedoya", "ledys@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "LedysBedoya.jpg", UserType.User);
            await CheckUserAsync("Brad", "Pitt", "brad@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "Brad.jpg", UserType.User);
            await CheckUserAsync("Angelina", "Jolie", "angelina@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "Angelina.jpg", UserType.User);
            await CheckUserAsync("Bob", "Marley", "bob@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "bob.jpg", UserType.User);
            await CheckUserAsync("Celia", "Cruz", "celia@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "celia.jpg", UserType.Admin);
            await CheckUserAsync("Fredy", "Mercury", "fredy@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "fredy.jpg", UserType.User);
            await CheckUserAsync("Hector", "Lavoe", "hector@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "hector.jpg", UserType.User);
            await CheckUserAsync("Liv", "Taylor", "liv@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "liv.jpg", UserType.User);
            await CheckUserAsync("Otep", "Shamaya", "otep@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "otep.jpg", UserType.User);
            await CheckUserAsync("Ozzy", "Osbourne", "ozzy@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "ozzy.jpg", UserType.User);
            await CheckUserAsync("Selena", "Quintanilla", "selenba@yopmail.com", "+964", "322 311 4620", "babil 40 strret hilla", "selena.jpg", UserType.User);
        }

        private async Task<User> CheckUserAsync(string firstName, string lastName, string email, string countryCode,
            string phoneNumber, string address, string image, UserType userType)
        {
            var user = await _usersUnitOfWork.GetUserAsync(email);
            if (user == null)
            {
                var city = await _context.cities.FirstOrDefaultAsync(x => x.Name == "Hillah");
                city ??= await _context.cities.FirstOrDefaultAsync();
                string filePath;
                if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
                {
                    filePath = $"{Environment.CurrentDirectory}\\images\\users\\{image}";
                }
                else
                {
                    filePath = $"{Environment.CurrentDirectory}/images/users/{image}";
                }

                var fileBytes = File.ReadAllBytes(filePath);
                var imagePath = await _fileStorage.SaveFileAsync(fileBytes, ".jpg", "users");

                user = new User
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    UserName = email,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    City = city,
                    UserType = userType,
                    Photo = imagePath,
                    CountryCode = "+964"
                };

                await _usersUnitOfWork.AddUserAsync(user, "123456");
                await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

                var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
                await _usersUnitOfWork.ConfirmEmailAsync(user, token);
            }

            return user;
        }

        private async Task CheckCategoriesAsync()
        {
            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>
                        {
                            new CategoryTranslation { Language = "en", Name = "Apple" },
                            new CategoryTranslation { Language = "ar", Name = "آبل" }
                        },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                            {
                                SubcategoryTranslations = new List<SubcategoryTranslation>
                                {
                                    new SubcategoryTranslation { Language = "en", Name = "iPhone 16" },
                                    new SubcategoryTranslation { Language = "ar", Name = "ايفون 16" },
                                    new SubcategoryTranslation { Language = "en", Name = "MacBook Pro" },
                                    new SubcategoryTranslation { Language = "ar", Name = "ماك بوك برو" },
                                    new SubcategoryTranslation { Language = "en", Name = "iPad Air" },
                                    new SubcategoryTranslation { Language = "ar", Name = "ايباد اير" },

                                },
                            }
                    }
                });

                _context.Categories.Add(new Category
                {

                    categoryTranslations = new List<CategoryTranslation>
                        {
                            new CategoryTranslation { Language = "en", Name = "Cars" },
                            new CategoryTranslation { Language = "ar", Name = "السيارات" }
                        },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                       SubcategoryTranslations = new List<SubcategoryTranslation>
                        {

                            new SubcategoryTranslation { Language = "en", Name = "Electric Cars" },
                            new SubcategoryTranslation { Language = "ar", Name = "سيارات كهربائية" },
                            new SubcategoryTranslation { Language = "en", Name = "Luxury Cars" },
                            new SubcategoryTranslation { Language = "ar", Name = "سيارات فاخرة" },
                            new SubcategoryTranslation { Language = "en", Name = "SUV" },
                            new SubcategoryTranslation { Language = "ar", Name = "دفع رباعي" }
                        }
                    }
                  }
                }
                );

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Beauty" },
                       new CategoryTranslation { Language = "ar", Name = "الجمال" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {

                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Skincare" },
                                new SubcategoryTranslation { Language = "ar", Name = "العناية بالبشرة" },
                                new SubcategoryTranslation { Language = "en", Name = "Makeup" },
                                new SubcategoryTranslation { Language = "ar", Name = "مكياج" },
                                new SubcategoryTranslation { Language = "en", Name = "Haircare" },
                                new SubcategoryTranslation { Language = "ar", Name = "العناية بالشعر" }

                        }
                    }

                }
                });

                _context.Categories.Add(new Category
                {

                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Footwear" },
                       new CategoryTranslation { Language = "ar", Name = "الأحذية" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name="Men's Shoes" },
                                new SubcategoryTranslation { Language = "ar", Name="أحذية رجالية" },
                                new SubcategoryTranslation { Language = "en", Name="Women's Shoes" },
                                new SubcategoryTranslation { Language = "ar", Name="أحذية نسائية" },
                            }

                        }
                    }
                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Food" },
                       new CategoryTranslation { Language = "ar", Name = "طعام" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Snacks" },
                                new SubcategoryTranslation { Language = "ar", Name = "وجبات خفيفة" },
                                new SubcategoryTranslation { Language = "en", Name = "Beverages" },
                                new SubcategoryTranslation { Language = "ar", Name = "مشروبات" },
                                new SubcategoryTranslation { Language = "en", Name = "Frozen Foods" },
                                new SubcategoryTranslation { Language = "ar", Name = "أطعمة مجمدة" }
                            }
                        }
                    }

                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Cosmetics" },
                       new CategoryTranslation { Language = "ar", Name = "مستحضرات التجميل" }
                   },
                    subcategories = new List<Subcategory>
                    {

                        new Subcategory
                        {
                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Lipstick" },
                                new SubcategoryTranslation { Language = "ar", Name = "أحمر شفاه" },
                                new SubcategoryTranslation { Language = "en", Name = "Foundation" },
                                new SubcategoryTranslation { Language = "ar", Name = "كريم أساس" }
                            }
                        }
                    }
                });

                _context.Categories.Add(new Category
                {

                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Sports" },
                       new CategoryTranslation { Language = "ar", Name = "الرياضة" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Fitness Equipment" },
                                new SubcategoryTranslation { Language = "ar", Name = "معدات اللياقة البدنية" },
                                new SubcategoryTranslation { Language = "en", Name = "Outdoor Sports" },
                                new SubcategoryTranslation { Language = "ar", Name = "الرياضات الخارجية" }
                            }
                        }
                    }

                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Gaming" },
                       new CategoryTranslation { Language = "ar", Name = "الألعاب" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations=new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Consoles" },
                                new SubcategoryTranslation { Language = "ar", Name = "أجهزة الألعاب" },
                                new SubcategoryTranslation { Language = "en", Name = "Accessories" },
                                new SubcategoryTranslation { Language = "ar", Name = "ملحقات" }
                            }
                        }
                    }

                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Toys" },
                       new CategoryTranslation { Language = "ar", Name = "لعب اطفال" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations=new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Educational Toys" },
                                new SubcategoryTranslation { Language = "ar", Name = "ألعاب تعليمية" },
                                new SubcategoryTranslation { Language = "en", Name = "Action Figures" },
                                new SubcategoryTranslation { Language = "ar", Name = "شخصيات الحركة" }
                            }
                        }
                    }

                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Pets" },
                       new CategoryTranslation { Language = "ar", Name = "حيوانات أليفة" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {
                            SubcategoryTranslations=new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Pet Food" },
                                new SubcategoryTranslation { Language = "ar", Name = "طعام الحيوانات الأليفة" },
                                new SubcategoryTranslation { Language = "en", Name = "Pet Accessories" },
                                new SubcategoryTranslation { Language = "ar", Name = "إكسسوارات الحيوانات الأليفة" }
                            }

                        }
                    }
                });

                _context.Categories.Add(new Category
                {

                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Nutrition" },
                       new CategoryTranslation { Language = "ar", Name = "تَغذِيَة" }
                   },

                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {

                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Supplements" },
                                new SubcategoryTranslation { Language = "ar", Name = "مُكَمِّلَات" },
                                new SubcategoryTranslation { Language = "en", Name = "Vitamins" },
                                new SubcategoryTranslation { Language = "ar", Name = "فِيتَامِين" }
                            }
                        }
                    }
                });

                _context.Categories.Add(new Category
                {

                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Clothing" },
                       new CategoryTranslation { Language = "ar", Name = "ملابس" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {

                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name="Men's Clothing" },
                                new SubcategoryTranslation { Language = "ar", Name="ملابس رجالية" },
                                new SubcategoryTranslation { Language = "en", Name="Women's Clothing" },
                                new SubcategoryTranslation { Language = "ar", Name="ملابس نسائية" }
                            }
                        }
                    }

                });

                _context.Categories.Add(new Category
                {
                    categoryTranslations = new List<CategoryTranslation>()
                   {
                       new CategoryTranslation { Language = "en", Name = "Technology" },
                       new CategoryTranslation { Language = "ar", Name = "تكنولوجيا" }
                   },
                    subcategories = new List<Subcategory>
                    {
                        new Subcategory
                        {

                            SubcategoryTranslations = new List<SubcategoryTranslation>
                            {
                                new SubcategoryTranslation { Language = "en", Name = "Laptops" },
                                new SubcategoryTranslation { Language = "ar", Name = "أجهزة الكمبيوتر المحمولة" },
                                new SubcategoryTranslation { Language = "en", Name = "Smartphones" },
                                new SubcategoryTranslation { Language = "ar", Name = "الهواتف الذكية" }
                            }
                        }
                    }

                });

                await _context.SaveChangesAsync();
            }
        }



        private async Task CheckProductsAsync()
        {
            if (!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, ["Men's Shoes", "Fitness Equipment"], ["adidas_barracuda.png"], "Adidas", 230000M, 0.3M);
                await AddProductAsync("Adidas Superstar", 250000M, ["Men's Shoes", "Fitness Equipment"], ["Adidas_superstar.png"], "Adidas", 200000M, 0.3M);
                await AddProductAsync("Avocado", 5000M, ["Snacks"], ["Aguacate1.jpg", "Aguacate2.jpg", "Aguacate3.jpg"], "Nature Made", 4000M, 0.3M);
                await AddProductAsync("AirPods", 1300000M, ["Smartphones", "iPhone 16"], ["airpos.png", "airpos2.png"], "Apple", 1000000M, 0.3M);
                await AddProductAsync("Akai APC40 MKII", 2650000M, ["Laptops"], ["Akai1.jpg", "Akai2.jpg", "Akai3.jpg"], "Akai", 2150000M, 0.3M);
                await AddProductAsync("Apple Watch Ultra", 4500000M, ["iPhone 16", "Smartphones"], ["AppleWatchUltra1.jpg", "AppleWatchUltra2.jpg"], "Apple", 4000000M, 0.3M);
                await AddProductAsync("Bose Headphones", 870000M, ["Smartphones"], ["audifonos_bose.png"], "Bose", 800000M, 0.3M);
                await AddProductAsync("Ribble Bicycle", 12000000M, ["Outdoor Sports"], ["bicicleta_ribble.png"], "Ribble", 10000000M, 0.3M);
                await AddProductAsync("Plaid Shirt", 56000M, ["Men's Clothing"], ["camisa_cuadros.png"], "H&M", 50000M, 0.3M);
                await AddProductAsync("Bicycle Helmet", 820000M, ["Outdoor Sports"], ["casco_bicicleta.png", "casco.png"], "Outward Hound", 750000M, 0.3M);
                await AddProductAsync("Sports Glasses", 160000M, ["Outdoor Sports"], ["Gafas1.jpg", "Gafas2.jpg"], "Columbia", 130000M, 0.3M);
                await AddProductAsync("Triple Meat Burger", 25500M, ["Snacks"], ["Hamburguesa1.jpg", "Hamburguesa2.jpg", "Hamburguesa3.jpg"], "Nestle", 16500M, 0.3M);
                await AddProductAsync("iPad", 2300000M, ["iPad Air", "Smartphones"], ["ipad.png"], "Apple", 200000M, 0.3M);
                await AddProductAsync("iPhone 13", 5200000M, ["iPhone 16", "Smartphones"], ["iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png"], "Apple", 4900000M, 0.3M);
                await AddProductAsync("MacBook Pro", 12100000M, ["MacBook Pro", "Laptops"], ["mac_book_pro.png"], "Apple", 11500000M, 0.3M);
                await AddProductAsync("Dumbbells", 370000M, ["Fitness Equipment"], ["mancuernas.png"], "Bowflex", 300000M, 0.3M);
                await AddProductAsync("Face Mask", 26000M, ["Skincare"], ["mascarilla_cara.png"], "Nivea", 20000M, 0.3M);
                await AddProductAsync("New Balance 530", 180000M, ["Men's Shoes", "Fitness Equipment"], ["newbalance530.png"], "New Balance", 140000M, 0.3M);
                await AddProductAsync("New Balance 565", 179000M, ["Men's Shoes", "Fitness Equipment"], ["newbalance565.png"], "New Balance", 155000M, 0.3M);
                await AddProductAsync("Nike Air", 233000M, ["Men's Shoes", "Fitness Equipment"], ["nike_air.png"], "Nike", 200000M, 0.3M);
                await AddProductAsync("Nike Zoom", 249900M, ["Men's Shoes", "Fitness Equipment"], ["nike_zoom.png"], "Nike", 200000M, 0.3M);
                await AddProductAsync("Adidas Women's Sweatshirt", 134000M, ["Women's Clothing", "Fitness Equipment"], ["buso_adidas.png"], "Adidas", 100000M, 0.3M);
                await AddProductAsync("Boost Original Supplement", 15600M, ["Supplements"], ["Boost_Original.png"], "Optimum Nutrition", 150000M, 0.3M);
                await AddProductAsync("Whey Protein", 252000M, ["Supplements"], ["whey_protein.png"], "MuscleTech", 200000M, 0.3M);
                await AddProductAsync("Pet Harness", 25000M, ["Pet Accessories"], ["arnes_mascota.png"], "KONG", 20000M, 0.3M);
                await AddProductAsync("Pet Bed", 99000M, ["Pet Accessories"], ["cama_mascota.png"], "Outward Hound", 78000M, 0.3M);
                await AddProductAsync("Gamer Keyboard", 67000M, ["Accessories", "Laptops"], ["teclado_gamer.png"], "Razer", 53000M, 0.3M);
                await AddProductAsync("Luxury Ring 17", 1600000M, ["Luxury Cars"], ["Ring1.jpg", "Ring2.jpg"], "BMW", 1350000M, 0.3M);
                await AddProductAsync("Gamer Chair", 980000M, ["Accessories", "Laptops"], ["silla_gamer.png"], "Logitech", 715000M, 0.3M);
                await AddProductAsync("Gamer Mouse", 132000M, ["Accessories", "Laptops"], ["mouse_gamer.png"], "Logitech", 99900M, 0.3M);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(string name, decimal price, List<string> Subcategories, List<string> images, string brandNames,
            decimal cost, decimal desiredProfit)
        {
            var barcode = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();           
            Product product = new()
            {
                Description = name,
                Name = name,
                Barcode = barcode,
                Cost = cost,
                Price = price,
                Stock = 10,
                DesiredProfit = desiredProfit,
                CreatedAt=DateTime.UtcNow,
                serialNumbers = new List<SerialNumber>(),
                HasSerial = true,
                ProductImages = new List<ProductImage>(),
                productsubCategories = new List<ProductsubCategory>(),             

            };

            foreach (var subcategoryName in Subcategories)
            {
                var subcategory = await _context.subcategories
                                         .Include(sc => sc.SubcategoryTranslations)
                                         .FirstOrDefaultAsync(sc => sc.SubcategoryTranslations!
                                         .Any(t => t.Name == subcategoryName));
                if (subcategory == null)
                {
                    Console.WriteLine($"⚠️ Subcategory '{subcategoryName}' not found in database!");
                    continue;
                }
                if (subcategory != null)
                {
                    product.productsubCategories.Add(new ProductsubCategory { Category = subcategory });

                }
                var brand = await _context.brands.Include(b=>b.BrandTranslations).FirstOrDefaultAsync(b => b.BrandTranslations!.Any(t=>t.Name==brandNames));
                if (brand == null)
                {
                    brand = new Brand {  SubcategoryId=subcategory!.Id, Subcategory=subcategory, 
                        BrandTranslations =new List<BrandTranslation>
                        {
                            new BrandTranslation { Language="en", Name=brandNames }
                        }
                    };
                }
                product.brand = brand;
            }
                foreach (string image in images)
                {
                    string filePath = _runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows)
                        ? $"{Environment.CurrentDirectory}\\images\\products\\{image}"
                        : $"{Environment.CurrentDirectory}/images/products/{image}";

                    var fileBytes = File.ReadAllBytes(filePath);
                    var imagePath = await _fileStorage.SaveFileAsync(fileBytes, ".png", "products");

                    product.ProductImages.Add(new ProductImage { Image = imagePath });
                }
                for (int i = 0; i < 10; i++)
                {
                    product.serialNumbers.Add(new SerialNumber
                    {
                        SerialNumberValue = barcode,
                        SerialStatus = Share.Enums.SerialStatus.Used,
                        PurchaseDate = DateTime.Now,
                    });
                }
           
           
            _context.Products.Add(product);
            }

        
        private async Task CheckCountriesAsync()
        {
            if (!_context.countries.Any())
            {
                var countries = new List<Country>
            {
                new Country
                {
                    Name = "Iraq",
                    states = new List<State>
                    {
                        new State
                        {
                            Name = "Baghdad",
                            cities = new List<City>
                            {
                                new City { Name = "Al-Adel" },
                                new City { Name = "Karrada" },
                                new City { Name = "Al-Mansour" }
                            }
                        },
                        new State
                        {
                            Name = "Basra",
                            cities = new List<City>
                            {
                                new City { Name = "Al-Ashar" },
                                new City { Name = "Al-Jubaila" }
                            }
                        },
                         new State
                        {
                            Name = "Babylon",
                            cities = new List<City>
                            {
                                new City { Name = "Hillah" },
                                new City { Name = "Bab Al Mashhad" }
                            }
                        }
                    }
                },

                new Country
                {
                    Name = "Egypt",
                    states = new List<State>
                    {
                        new State
                        {
                            Name = "Cairo",
                            cities = new List<City>
                            {
                                new City { Name = "Nasr City" },
                                new City { Name = "Heliopolis" }
                            }
                        },
                        new State
                        {
                            Name = "Alexandria",
                            cities = new List<City>
                            {
                                new City { Name = "Sidi Gaber" },
                                new City { Name = "Miami" }
                            }
                        }
                    }
                },

                new Country
                {
                    Name = "Saudi Arabia",
                    states = new List<State>
                    {
                        new State
                        {
                            Name = "Riyadh",
                            cities = new List<City>
                            {
                                new City { Name = "Al Olaya" },
                                new City { Name = "Al Malaz" }
                            }
                        },
                        new State
                        {
                            Name = "Jeddah",
                            cities = new List<City>
                            {
                                new City { Name = "Al Hamra" },
                                new City { Name = "Al Aziziyah" }
                            }
                        }
                    }
                },

                new Country
                {
                    Name = "Jordan",
                    states = new List<State>
                    {
                        new State
                        {
                            Name = "Amman",
                            cities = new List<City>
                            {
                                new City { Name = "Shmeisani" },
                                new City { Name = "Sweifieh" }
                            }
                        }
                    }
                },

                new Country
                {
                    Name = "UAE",
                    states = new List<State>
                    {
                        new State
                        {
                            Name = "Dubai",
                            cities = new List<City>
                            {
                                new City { Name = "Deira" },
                                new City { Name = "Jumeirah" }
                            }
                        },
                        new State
                        {
                            Name = "Abu Dhabi",
                            cities = new List<City>
                            {
                                new City { Name = "Khalifa City" },
                                new City { Name = "Al Reem Island" }
                            }
                        }
                    }
                }
            };

                    _context.countries.AddRange(countries);
                    await _context.SaveChangesAsync();
                }
            }
        private async Task CheckBrandAsunc()
        {
            if(!_context.brands.Any())
            {
                    var subcategory = await _context.subcategories
                                      .FirstOrDefaultAsync();
                    var brand = new Brand
                    {
                        SubcategoryId = subcategory!.Id,
                        Subcategory=subcategory,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Apple" },
                            new BrandTranslation { Language = "ar", Name = "آبل" }
                        }
                    };
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        Subcategory = subcategory,
                        BrandTranslations = new List<BrandTranslation>
                    {
                        new BrandTranslation { Language = "en", Name = "Samsung" },
                        new BrandTranslation { Language = "ar", Name = "سامسونج" }
                    }
                    };
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        Subcategory = subcategory,
                        BrandTranslations = new List<BrandTranslation>
                    {
                        new BrandTranslation { Language = "en", Name = "Xiaomi" },
                        new BrandTranslation { Language = "ar", Name = "شاومي" }
                    }
                    };               
            

               _context.brands.Add(brand);
                await _context.SaveChangesAsync();
            }
        }

    }
}
