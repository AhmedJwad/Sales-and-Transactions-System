using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Sale.Api.Data;
using Sale.Api.Helpers;
using Sale.Api.Repositories.Implementations;
using Sale.Api.Repositories.Interfaces;
using Sale.Api.UnitsOfWork.Implementations;
using Sale.Api.UnitsOfWork.Interfaces;
using Sale.Share.Entities;
using Scalar.AspNetCore;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
 .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); 

//.AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddOpenApi();
builder.Services.AddTransient<SeedDb>();
builder.Services.AddScoped<IRuntimeInformationWrapper, RuntimeInformationWrapper>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
builder.Services.AddScoped<ICategoriesUnitOfWork, CategoriesUnitOfWork>();
builder.Services.AddScoped<IFileStorage, FileStorage>();
builder.Services.AddScoped<IorderHelper, OrderHelpers>();
builder.Services.AddScoped<ISubcategoryRepository, SubcategoryRepository>();
builder.Services.AddScoped<iSubcategoriesUnitofWorks, SubcategoriesUnitofWorks>();
builder.Services.AddScoped<IbrandRepository, BrandRepository>();
builder.Services.AddScoped<IbrandUnitofWork, brandUnitofWork>();
builder.Services.AddScoped<IProductRepository, ProductsRepository>();
builder.Services.AddScoped<IProductsUnitofWork, ProductsUnitofWork>();
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();
builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();
builder.Services.AddScoped<IStatesRepository, StatesRepository>();
builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();
builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
builder.Services.AddScoped<IMailHelper, MailHelper>();
builder.Services.AddScoped<IUsersRepository, UsersRepository>();
builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();
builder.Services.AddScoped<IcolourRepository, ColourRepositorycs>();
builder.Services.AddScoped<IcolourUnitofWorks, ColourUnitofWork>();
builder.Services.AddScoped<IsizeRepository , SizeRepository>();
builder.Services.AddScoped<IsizeUnitofWorks, SizeUnitofWorks>();
builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
builder.Services.AddScoped<IorderUnitofWorks,OrderUnitofWorks>();
builder.Services.AddScoped<IexchangeRateRepository, ExchangeRateRepository>();
builder.Services.AddScoped<IexchangeRateUnitofWork, ExchangeRateUnitofWork>();
builder.Services.AddScoped<ICurrencyConverter, CurrencyConverter>();
builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
    x.SignIn.RequireConfirmedEmail = true;
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
    x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    x.Lockout.MaxFailedAccessAttempts = 3;
    x.Lockout.AllowedForNewUsers = true;

})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
        ClockSkew = TimeSpan.Zero
    });

var app = builder.Build();
SeedData(app);
void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDb>();
        service!.SeedAsync().Wait();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // for error swagger details
    app.MapOpenApi();
    app.UseSwaggerUI(options => options.SwaggerEndpoint("/openapi/v1.json", "Sale2025.api"));     
    app.MapScalarApiReference();
}


app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "images/products")),
    RequestPath = "/images/products"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "images/users")),
    RequestPath = "/images/users"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "images/categories")),
    RequestPath = "/images/categories"
});
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "images/subcategories")),
    RequestPath = "/images/subcategories"
});
app.UseCors(x => x          
           .AllowAnyMethod()
           .AllowAnyHeader()
           .SetIsOriginAllowed(origin => true)
           .AllowCredentials());
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
