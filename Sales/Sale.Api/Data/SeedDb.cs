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
            await CheckCountriesAsync();
            await CheckRolesAsync();
            await CheckUsersAsync();
            await CheckBrandAsunc();
            await CheckCurrenciesAsync();
            await CheckProductsAsync();
           
        }

        private async Task CheckCurrenciesAsync()
        {
            if (!_context.currencies.Any())
            {
                _context.currencies.AddRange(                   
                    new Currency { Code = "IQ", Name = "Iraqi Dinar" },
                    new Currency { Code = "USD", Name = "US Dollar" }
                );

                await _context.SaveChangesAsync();
            }
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
                await AddProductAsync(["Adidas Barracuda",  "Adidas Barracuda Shoes", "en", "أديداس باراكودا", "أحذية أديداس باراكودا", "ar"], ["270000", "207.69"], ["Men's Shoes", "Fitness Equipment"], ["adidas_barracuda.png"], "Adidas");
                //await AddProductAsync(["أديداس باراكودا", "أحذية أديداس باراكودا", "ar"], 270000M, ["Men's Shoes", "Fitness Equipment"], ["adidas_barracuda.png"], "Adidas", 230000M, 0.3M);
                await AddProductAsync(["Adidas Superstar", "Adidas Superstar Shoes", "en", "أديداس سوبر ستار", "أحذية أديداس سوبر ستار", "ar"], ["250000", "192.3077"], ["Men's Shoes", "Fitness Equipment"], ["Adidas_superstar.png"], "Adidas");
                //await AddProductAsync(["أديداس سوبر ستار", "أحذية أديداس سوبر ستار", "ar"], 250000M, ["Men's Shoes", "Fitness Equipment"], ["Adidas_superstar.png"], "Adidas", 200000M, 0.3M);
                await AddProductAsync(["Avocado", "Fresh Avocado Fruit", "en", "أفوكادو", "فاكهة أفوكادو طازجة", "ar"],["5000", "3.8461"], ["Snacks"], ["Aguacate1.jpg", "Aguacate2.jpg", "Aguacate3.jpg"], "Nature Made");
                //await AddProductAsync(["أفوكادو", "فاكهة أفوكادو طازجة", "ar"], 5000M, ["Snacks"], ["Aguacate1.jpg", "Aguacate2.jpg", "Aguacate3.jpg"], "Nature Made", 4000M, 0.3M);
                await AddProductAsync(["AirPods", "Apple AirPods Wireless Earbuds", "en", "آيربودز", "سماعات آبل اللاسلكية", "ar"], ["1300000", "1,000"], ["Smartphones", "iPhone 16"], ["airpos.png", "airpos2.png"], "Apple");
                //await AddProductAsync(["آيربودز", "سماعات آبل اللاسلكية", "ar"], 1300000M, ["Smartphones", "iPhone 16"], ["airpos.png", "airpos2.png"], "Apple", 1000000M, 0.3M);
                await AddProductAsync(["Akai APC40 MKII", "Akai MIDI Controller for Music Production", "en", "أكاي APC40 MKII", "جهاز تحكم MIDI من أكاي لإنتاج الموسيقى", "ar"], ["2650000", "2038.46"], ["Laptops"], ["Akai1.jpg", "Akai2.jpg", "Akai3.jpg"], "Akai");
                //await AddProductAsync(["أكاي APC40 MKII", "جهاز تحكم MIDI من أكاي لإنتاج الموسيقى", "ar"], 2650000M, ["Laptops"], ["Akai1.jpg", "Akai2.jpg", "Akai3.jpg"], "Akai", 2150000M, 0.3M);
                await AddProductAsync(["Apple Watch Ultra", "Apple Watch Ultra Smartwatch", "en", "ساعة آبل ألترا", "ساعة آبل الذكية ألترا", "ar"], ["4000000", "3076.923"], ["iPhone 16", "Smartphones"], ["AppleWatchUltra1.jpg", "AppleWatchUltra2.jpg"], "Apple");
               // await AddProductAsync(["ساعة آبل ألترا", "ساعة آبل الذكية ألترا", "ar"], 4500000M, ["iPhone 16", "Smartphones"], ["AppleWatchUltra1.jpg", "AppleWatchUltra2.jpg"], "Apple", 4000000M, 0.3M);
                await AddProductAsync(["Bose Headphones", "Bose Wireless Headphones", "en", "سماعات بوز", "سماعات بوز اللاسلكية", "ar"], ["870000", "669.2307"], ["Smartphones"], ["audifonos_bose.png"], "Bose");
                //await AddProductAsync(["سماعات بوز", "سماعات بوز اللاسلكية", "ar"], 870000M, ["Smartphones"], ["audifonos_bose.png"], "Bose", 800000M, 0.3M);
                await AddProductAsync(["Ribble Bicycle", "Ribble Road Bicycle for Outdoor Sports", "en", "دراجة ريبل", "دراجة ريبل للطريق والرياضات الخارجية", "ar"], ["120000","92.56"], ["Outdoor Sports"], ["bicicleta_ribble.png"], "Ribble");
                //await AddProductAsync(["دراجة ريبل", "دراجة ريبل للطريق والرياضات الخارجية", "ar"], 12000000M, ["Outdoor Sports"], ["bicicleta_ribble.png"], "Ribble", 10000000M, 0.3M);
                await AddProductAsync(["Plaid Shirt", "Men's Plaid Casual Shirt", "en", "قميص مربعات", "قميص كاجوال رجالي بنقشة مربعات", "ar"], ["65002", "55.32"], ["Men's Clothing"], ["camisa_cuadros.png"], "H&M");
                //await AddProductAsync(["قميص مربعات", "قميص كاجوال رجالي بنقشة مربعات", "ar"], 56000M, ["Men's Clothing"], ["camisa_cuadros.png"], "H&M", 50000M, 0.3M);
                await AddProductAsync(["Bicycle Helmet", "Protective Bicycle Helmet for Outdoor Sports", "en", "خوذة الدراجة", "خوذة حماية للدراجات للرياضات الخارجية", "ar"], ["8995522", "55.33"], ["Outdoor Sports"], ["casco_bicicleta.png", "casco.png"], "Outward Hound");
                //await AddProductAsync(["خوذة الدراجة", "خوذة حماية للدراجات للرياضات الخارجية", "ar"], 820000M, ["Outdoor Sports"], ["casco_bicicleta.png", "casco.png"], "Outward Hound", 750000M, 0.3M);
                await AddProductAsync(["Sports Glasses", "Protective Sports Glasses for Outdoor Activities", "en", "نظارات رياضية", "نظارات حماية للرياضات الخارجية", "ar"], ["8995555", "2235.22"], ["Outdoor Sports"], ["Gafas1.jpg", "Gafas2.jpg"], "Columbia");
                //await AddProductAsync(["نظارات رياضية", "نظارات حماية للرياضات الخارجية", "ar"], 160000M, ["Outdoor Sports"], ["Gafas1.jpg", "Gafas2.jpg"], "Columbia", 130000M, 0.3M);
                await AddProductAsync(["Triple Meat Burger", "Delicious Triple Meat Burger Snack", "en", "برجر اللحم الثلاثي", "برجر لذيذ من ثلاثة أنواع لحم", "ar"], ["8995522", "55.33"], ["Snacks"], ["Hamburguesa1.jpg", "Hamburguesa2.jpg", "Hamburguesa3.jpg"], "Nestle");
                //await AddProductAsync(["برجر اللحم الثلاثي", "برجر لذيذ من ثلاثة أنواع لحم", "ar"], 25500M, ["Snacks"], ["Hamburguesa1.jpg", "Hamburguesa2.jpg", "Hamburguesa3.jpg"], "Nestle", 16500M, 0.3M);
                await AddProductAsync(["iPad", "Apple iPad Tablet with Retina Display", "en", "آيباد", "جهاز آيباد من أبل بشاشة Retina", "ar"], ["8995555", "2235.22"], ["iPad Air", "Smartphones"], ["ipad.png"], "Apple");
                //await AddProductAsync(["آيباد", "جهاز آيباد من أبل بشاشة Retina", "ar"], 2300000M, ["iPad Air", "Smartphones"], ["ipad.png"], "Apple", 200000M, 0.3M);
                await AddProductAsync(["iPhone 13", "Apple iPhone 13 with A15 Bionic Chip", "en", "آيفون 13", "هاتف آيفون 13 من أبل بشريحة A15 Bionic", "ar"], ["8995555", "2235.22"], ["iPhone 16", "Smartphones"], ["iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png"], "Apple");
                //await AddProductAsync(["آيفون 13", "هاتف آيفون 13 من أبل بشريحة A15 Bionic", "ar"], 5200000M, ["iPhone 16", "Smartphones"], ["iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png"], "Apple", 49000, 0.3M);
                await AddProductAsync(["MacBook Pro", "Apple MacBook Pro Laptop with M1 Chip", "en", "ماك بوك برو", "حاسوب محمول ماك بوك برو من أبل بشريحة M1", "ar"], ["89955", "2235.22"], ["MacBook Pro", "Laptops"], ["mac_book_pro.png"], "Apple");
                //await AddProductAsync(["ماك بوك برو", "حاسوب محمول ماك بوك برو من أبل بشريحة M1", "ar"], 12100000M, ["MacBook Pro", "Laptops"], ["mac_book_pro.png"], "Apple", 11500000M, 0.3M);
                await AddProductAsync(["Dumbbells", "Set of adjustable dumbbells for home workouts", "en", "أثقال", "مجموعة أثقال قابلة للتعديل للتمارين المنزلية", "ar"], ["8995555", "2235.22"], ["Fitness Equipment"], ["mancuernas.png"], "Bowflex");
               // await AddProductAsync(["أثقال", "مجموعة أثقال قابلة للتعديل للتمارين المنزلية", "ar"], 370000M, ["Fitness Equipment"], ["mancuernas.png"], "Bowflex", 300000M, 0.3M);
                await AddProductAsync(["Face Mask", "Nourishing face mask for all skin types", "en", "قناع الوجه", "قناع وجه مغذي لجميع أنواع البشرة", "ar"], ["8995555", "2235.22"], ["Skincare"], ["mascarilla_cara.png"], "Nivea");
                //await AddProductAsync(["قناع الوجه", "قناع وجه مغذي لجميع أنواع البشرة", "ar"], 26000M, ["Skincare"], ["mascarilla_cara.png"], "Nivea", 20000M, 0.3M);
                await AddProductAsync(["New Balance 530", "Men's running shoes for everyday comfort", "en", "نيو بالانس 530", "حذاء جري رجالي للراحة اليومية", "ar"], ["8995555", "2235.22"], ["Men's Shoes", "Fitness Equipment"], ["newbalance530.png"], "New Balance");
               // await AddProductAsync(["نيو بالانس 530", "حذاء جري رجالي للراحة اليومية", "ar"], 180000M, ["Men's Shoes", "Fitness Equipment"], ["newbalance530.png"], "New Balance", 140000M, 0.3M);
                await AddProductAsync(["New Balance 565", "Men's sneakers for casual and sports use", "en", "نيو بالانس 565", "حذاء رياضي رجالي للاستخدام اليومي والرياضي", "ar"], ["8995555", "2235.22"], ["Men's Shoes", "Fitness Equipment"], ["newbalance565.png"], "New Balance");
                //await AddProductAsync(["نيو بالانس 565", "حذاء رياضي رجالي للاستخدام اليومي والرياضي", "ar"], 179000M, ["Men's Shoes", "Fitness Equipment"], ["newbalance565.png"], "New Balance", 155000M, 0.3M);
                await AddProductAsync(["Nike Air", "Comfortable men's sneakers for sports and casual wear", "en", "نايكي إير", "حذاء رياضي رجالي مريح للرياضة والاستخدام اليومي", "ar"], ["8995555", "2235.22"], ["Men's Shoes", "Fitness Equipment"], ["nike_air.png"], "Nike");
                //await AddProductAsync(["نايكي إير", "حذاء رياضي رجالي مريح للرياضة والاستخدام اليومي", "ar"], 233000M, ["Men's Shoes", "Fitness Equipment"], ["nike_air.png"], "Nike", 200000M, 0.3M);
                await AddProductAsync(["Nike Zoom", "High-performance men's running shoes", "en", "نايكي زووم", "حذاء عداء رجالي عالي الأداء", "ar"], ["8995555", "2235.22"], ["Men's Shoes", "Fitness Equipment"], ["nike_zoom.png"], "Nike");
                //await AddProductAsync(["نايكي زووم", "حذاء عداء رجالي عالي الأداء", "ar"], 249900M, ["Men's Shoes", "Fitness Equipment"], ["nike_zoom.png"], "Nike", 200000M, 0.3M);
                await AddProductAsync(["Adidas Women's Sweatshirt", "Comfortable and stylish women's sweatshirt", "en", "سويت شيرت أديداس للنساء", "سويت شيرت مريح وأنيق للنساء", "ar"], ["8995555", "2235.22"], ["Women's Clothing", "Fitness Equipment"], ["buso_adidas.png"], "Adidas");
               // await AddProductAsync(["سويت شيرت أديداس للنساء", "سويت شيرت مريح وأنيق للنساء", "ar"], 134000M, ["Women's Clothing", "Fitness Equipment"], ["buso_adidas.png"], "Adidas", 100000M, 0.3M);
                await AddProductAsync(["Boost Original Supplement", "Nutritious protein supplement for energy", "en", "مكمل Boost الأصلي", "مكمل بروتين مغذي للطاقة", "ar"], ["8995555", "2235.22"], ["Supplements"], ["Boost_Original.png"], "Optimum Nutrition");
               // await AddProductAsync(["مكمل Boost الأصلي", "مكمل بروتين مغذي للطاقة", "ar"], 15600M, ["Supplements"], ["Boost_Original.png"], "Optimum Nutrition", 150000M, 0.3M);
                await AddProductAsync(["Whey Protein", "High-quality protein powder for muscle growth", "en", "بروتين واي", "مسحوق بروتين عالي الجودة لنمو العضلات", "ar"], ["8995555", "2235.22"], ["Supplements"], ["whey_protein.png"], "MuscleTech");
               //await AddProductAsync(["بروتين واي", "مسحوق بروتين عالي الجودة لنمو العضلات", "ar"], 252000M, ["Supplements"], ["whey_protein.png"], "MuscleTech", 200000M, 0.3M);
                await AddProductAsync(["Pet Harness", "Durable harness for small and medium pets", "en", "حزام للحيوانات الأليفة", "حزام متين للحيوانات الصغيرة والمتوسطة", "ar"], ["8995555", "2235.22"], ["Pet Accessories"], ["arnes_mascota.png"], "KONG");
                //await AddProductAsync(["حزام للحيوانات الأليفة", "حزام متين للحيوانات الصغيرة والمتوسطة", "ar"], 25000M, ["Pet Accessories"], ["arnes_mascota.png"], "KONG", 20000M, 0.3M);
                await AddProductAsync(["Pet Bed", "Comfortable bed for pets", "en", "سرير للحيوانات الأليفة", "سرير مريح للحيوانات الأليفة", "ar"], ["8995555", "2235.22"], ["Pet Accessories"], ["cama_mascota.png"], "Outward Hound");
               // await AddProductAsync(["سرير للحيوانات الأليفة", "سرير مريح للحيوانات الأليفة", "ar"], 99000M, ["Pet Accessories"], ["cama_mascota.png"], "Outward Hound", 78000M, 0.3M);
                await AddProductAsync(["Gamer Keyboard", "High-performance keyboard for gaming", "en", "لوحة مفاتيح للألعاب", "لوحة مفاتيح عالية الأداء للألعاب", "ar"], ["8995555", "2235.22"], ["Accessories", "Laptops"], ["teclado_gamer.png"], "Razer");
               // await AddProductAsync(["لوحة مفاتيح للألعاب", "لوحة مفاتيح عالية الأداء للألعاب", "ar"], 67000M, ["Accessories", "Laptops"], ["teclado_gamer.png"], "Razer", 53000M, 0.3M);
                await AddProductAsync(["Luxury Ring 17", "Elegant luxury ring", "en", "خاتم فاخر 17", "خاتم فاخر وأنيق", "ar"], ["8995555", "2235.22"], ["Luxury Cars"], ["Ring1.jpg", "Ring2.jpg"], "BMW");
               // await AddProductAsync(["خاتم فاخر 17", "خاتم فاخر وأنيق", "ar"], 1600000M, ["Luxury Cars"], ["Ring1.jpg", "Ring2.jpg"], "BMW", 1350000M, 0.3M);
                await AddProductAsync(["Gamer Chair", "Comfortable gaming chair", "en", "كرسي ألعاب", "كرسي ألعاب مريح", "ar"], ["8995555", "2235.22"], ["Accessories", "Laptops"], ["silla_gamer.png"], "Logitech");
               // await AddProductAsync(["كرسي ألعاب", "كرسي ألعاب مريح", "ar"], 980000M, ["Accessories", "Laptops"], ["silla_gamer.png"], "Logitech", 715000M, 0.3M);
                await AddProductAsync(["Gamer Mouse", "High precision gaming mouse", "en", "فأرة ألعاب", "فأرة ألعاب عالية الدقة", "ar"], ["8995555", "2235.22"], ["Accessories", "Laptops"], ["mouse_gamer.png"], "Logitech");
                //await AddProductAsync(["فأرة ألعاب", "فأرة ألعاب عالية الدقة", "ar"], 132000M, ["Accessories", "Laptops"], ["mouse_gamer.png"], "Logitech", 99900M, 0.3M);

                await _context.SaveChangesAsync();
            }
        }

        private async Task AddProductAsync(List<string>Producttranslation, List<string> prices, List<string> Subcategories, List<string> images, string brandNames)
        {
            var barcode = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();           
            Product product = new()
            {              
                Barcode = barcode,               
                Stock = 10,              
                CreatedAt=DateTime.UtcNow,
                serialNumbers = new List<SerialNumber>(),
                HasSerial = true,
                ProductImages = new List<ProductImage>(),
                productsubCategories = new List<ProductsubCategory>(),  
                ProductTranslations=new List<ProductTranslation>(),
                ProductPrices=new List<ProductPrice>(),
            };
            for (int i = 0; i < Producttranslation.Count; i += 6)
            {
                product.ProductTranslations.Add(new ProductTranslation
                {
                    Name = Producttranslation[i],
                    Description = Producttranslation[i + 1],
                    Language = Producttranslation[i + 2]                     
                });
                product.ProductTranslations.Add(new ProductTranslation
                {
                    Name = Producttranslation[i+3],
                    Description = Producttranslation[i + 4],
                    Language = Producttranslation[i + 5]
                });
            }

            foreach (var subcategoryName in Subcategories)
            {
                //var subcategory = await _context.subcategories
                //                         .Include(sc => sc.SubcategoryTranslations)
                //                         .FirstOrDefaultAsync(sc => sc.SubcategoryTranslations!
                //                         .Any(t => t.Name == subcategoryName));
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
                
            }
            var brand = await _context.brands.Include(b => b.BrandTranslations).FirstOrDefaultAsync(b => b.BrandTranslations!.Any(t => t.Name == brandNames));
            var subcategorybrand = await _context.subcategories.Include(t => t.SubcategoryTranslations).FirstOrDefaultAsync();
            if (brand == null)
            {
                brand = new Brand
                {
                    SubcategoryId = subcategorybrand!.Id,
                    Subcategory = subcategorybrand,
                    BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language="en", Name=brandNames },
                            new BrandTranslation { Language="ar", Name=brandNames }
                        }
                };

            }

            product.brand = brand;
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
            var currencies = await _context.currencies.ToListAsync();
            for (int i = 0; i < prices.Count; i++)
            {
                product.ProductPrices.Add(new ProductPrice
                {
                    CurrencyId= currencies[i].Id,
                    Price = decimal.Parse(prices[i]),
                    CreatedAt=DateTime.UtcNow,
                    Cost= decimal.Parse(prices[i]),
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
            if (!_context.brands.Any())
            {
                var subcategory = await _context.subcategories.FirstOrDefaultAsync();

                var brands = new List<Brand>
                {
                    new Brand
                    {
                        SubcategoryId = subcategory!.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Apple" },
                            new BrandTranslation { Language = "ar", Name = "آبل" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Samsung" },
                            new BrandTranslation { Language = "ar", Name = "سامسونج" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Xiaomi" },
                            new BrandTranslation { Language = "ar", Name = "شاومي" }
                        }
                    },

                    // البراندات التي طلبتها
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Adidas" },
                            new BrandTranslation { Language = "ar", Name = "أديداس" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Nature Made" },
                            new BrandTranslation { Language = "ar", Name = "نيتشر ميد" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Akai" },
                            new BrandTranslation { Language = "ar", Name = "أكاي" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Bose" },
                            new BrandTranslation { Language = "ar", Name = "بوز" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Ribble" },
                            new BrandTranslation { Language = "ar", Name = "ريبّل" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "H&M" },
                            new BrandTranslation { Language = "ar", Name = "اتش آند ام" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Outward Hound" },
                            new BrandTranslation { Language = "ar", Name = "اوتوارد هاوند" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Columbia" },
                            new BrandTranslation { Language = "ar", Name = "كولومبيا" }
                        }
                    },
                    new Brand
                    {
                        SubcategoryId = subcategory.Id,
                        BrandTranslations = new List<BrandTranslation>
                        {
                            new BrandTranslation { Language = "en", Name = "Nestle" },
                            new BrandTranslation { Language = "ar", Name = "نستله" }
                        }
                    }
                };

                _context.brands.AddRange(brands);
                await _context.SaveChangesAsync();
            }
        }


    }

}

