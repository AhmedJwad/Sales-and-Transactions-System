using Microsoft.EntityFrameworkCore;
using Sale.Api.Helpers;
using Sale.Share.Entities;
using System.Runtime.InteropServices;

namespace Sale.Api.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IFileStorage _fileStorage;
        private readonly IRuntimeInformationWrapper _runtimeInformationWrapper;

        public SeedDb(DataContext context, IFileStorage fileStorage, IRuntimeInformationWrapper runtimeInformationWrapper)
        {
            _context = context;
            _fileStorage = fileStorage;
            _runtimeInformationWrapper = runtimeInformationWrapper;
        }
        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCategoriesAsync();
            await CheckProductsAsync();
            await CheckCountriesAsync();

        }

        
        private async Task CheckCategoriesAsync()
        {

            if (!_context.Categories.Any())
            {
                _context.Categories.Add(new Category
                {
                    Name = "Apple",
                    subcategories =
                    [
                        new Subcategory { Name = "iPhone 16", Brands = [ new Brand { Name = "Apple" } ] },
                        new Subcategory { Name = "MacBook Pro", Brands = [ new Brand { Name = "Apple" } ] },
                        new Subcategory { Name = "iPad Air", Brands = [ new Brand { Name = "Apple" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Cars",
                    subcategories =
                    [
                        new Subcategory { Name = "Electric Cars", Brands = [ new Brand { Name = "Tesla" }, new Brand { Name = "Lucid" } ] },
                        new Subcategory { Name = "Luxury Cars", Brands = [ new Brand { Name = "BMW" }, new Brand { Name = "Mercedes-Benz" } ] },
                        new Subcategory { Name = "SUV", Brands = [ new Brand { Name = "Toyota" }, new Brand { Name = "Ford" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Beauty",
                    subcategories =
                    [
                        new Subcategory { Name = "Skincare", Brands = [ new Brand { Name = "Nivea" }, new Brand { Name = "L'Oreal" } ] },
                        new Subcategory { Name = "Makeup", Brands = [ new Brand { Name = "Maybelline" }, new Brand { Name = "MAC" } ] },
                        new Subcategory { Name = "Haircare", Brands = [ new Brand { Name = "Pantene" }, new Brand { Name = "Dove" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Footwear",
                    subcategories =
                    [
                        new Subcategory { Name = "Men's Shoes", Brands = [ new Brand { Name = "Nike" }, new Brand { Name = "Adidas" } ] },
                        new Subcategory { Name = "Women's Shoes", Brands = [ new Brand { Name = "Puma" }, new Brand { Name = "Reebok" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Food",
                    subcategories =
                    [
                        new Subcategory { Name = "Snacks", Brands = [ new Brand { Name = "Lay's" }, new Brand { Name = "Doritos" } ] },
                        new Subcategory { Name = "Beverages", Brands = [ new Brand { Name = "Pepsi" }, new Brand { Name = "Coca-Cola" } ] },
                        new Subcategory { Name = "Frozen Foods", Brands = [ new Brand { Name = "Nestle" }, new Brand { Name = "McCain" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Cosmetics",
                    subcategories =
                    [
                        new Subcategory { Name = "Lipstick", Brands = [ new Brand { Name = "Revlon" }, new Brand { Name = "Sephora" } ] },
                        new Subcategory { Name = "Foundation", Brands = [ new Brand { Name = "Estee Lauder" }, new Brand { Name = "Fenty Beauty" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Sports",
                    subcategories =
                    [
                        new Subcategory { Name = "Fitness Equipment", Brands = [ new Brand { Name = "NordicTrack" }, new Brand { Name = "Bowflex" } ] },
                        new Subcategory { Name = "Outdoor Sports", Brands = [ new Brand { Name = "Columbia" }, new Brand { Name = "The North Face" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Gaming",
                    subcategories =
                    [
                        new Subcategory { Name = "Consoles", Brands = [ new Brand { Name = "Sony" }, new Brand { Name = "Microsoft" } ] },
                         new Subcategory { Name = "Accessories", Brands = [ new Brand { Name = "Razer" }, new Brand { Name = "Logitech" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Toys",
                    subcategories =
                    [
                        new Subcategory { Name = "Educational Toys", Brands = [ new Brand { Name = "LeapFrog" }, new Brand { Name = "Fisher-Price" } ] },
                        new Subcategory { Name = "Action Figures", Brands = [ new Brand { Name = "Hasbro" }, new Brand { Name = "Mattel" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Pets",
                    subcategories =
                    [
                        new Subcategory { Name = "Pet Food", Brands = [ new Brand { Name = "Purina" }, new Brand { Name = "Pedigree" } ] },
                        new Subcategory { Name = "Pet Accessories", Brands = [ new Brand { Name = "KONG" }, new Brand { Name = "Outward Hound" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Nutrition",
                    subcategories =
                    [
                        new Subcategory { Name = "Supplements", Brands = [ new Brand { Name = "Optimum Nutrition" }, new Brand { Name = "MuscleTech" } ] },
                        new Subcategory { Name = "Vitamins", Brands = [ new Brand { Name = "Nature Made" }, new Brand { Name = "Centrum" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Clothing",
                    subcategories =
                    [
                        new Subcategory { Name = "Men's Clothing", Brands = [ new Brand { Name = "Levi's" }, new Brand { Name = "H&M" } ] },
                        new Subcategory { Name = "Women's Clothing", Brands = [ new Brand { Name = "Zara" }, new Brand { Name = "Forever 21" } ] }
                    ]
                });

                _context.Categories.Add(new Category
                {
                    Name = "Technology",
                    subcategories =
                    [
                        new Subcategory { Name = "Laptops", Brands = [ new Brand { Name = "Dell" }, new Brand { Name = "HP" } ] },
                        new Subcategory { Name = "Smartphones", Brands = [ new Brand { Name = "Samsung" }, new Brand { Name = "Xiaomi" } ] }
                    ]
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
                serialNumbers = new List<SerialNumber>(),
                HasSerial = true,
                ProductImages = new List<ProductImage>(),
                productsubCategories = new List<ProductsubCategory>(),

            };

            foreach (var subcategoryName in Subcategories)
            {
                var subcategory = await _context.subcategories.FirstOrDefaultAsync(sc => sc.Name == subcategoryName);
                if (subcategory != null)
                {
                    product.productsubCategories.Add(new ProductsubCategory { Category = subcategory });
                    var brand = await _context.brands.FirstOrDefaultAsync(b => b.Name == brandNames);
                    if (brand == null)
                    {
                        brand = new Brand
                        {
                            Name = brandNames,
                            Subcategory = subcategory
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

    }
}
