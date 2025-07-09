using FluentValidation;
using FluentValidation.AspNetCore;
using InventoryService.Api.Filters;
using InventoryService.Application;
using InventoryService.Application.Common.Behaviors;
using InventoryService.Application.Customers;
using InventoryService.Application.Mapping;
using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Entities.Identity;
using InventoryService.Domain.Entities.Products;
using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using InventoryService.Domain.Interfaces.Pricing;
using InventoryService.Infrastructure.Common;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using InventoryService.Infrastructure.Services;
using InventoryService.Infrastructure.Services.Pricing;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<CreateCustomerCommandValidator>();

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

builder.Services.AddTransient<IDiscountPolicy, PercentageDiscountPolicy>();
builder.Services.AddTransient<IVATPolicy, StandardVATPolicy>();

builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
});

builder.Services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
builder.Services.AddScoped<ITokenService, TokenService>();

//builder.Services.AddDbContext<InventoryDbContext>(options =>
//    options.UseInMemoryDatabase("InventoryDb"));

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISaleDetailsRepository, SaleDetailsRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSingleton<SalesConcurrencyFilter>();

//builder.Services.Configure<JwtSettings>(
//    builder.Configuration.GetSection("Jwt"));

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();

if (jwtSettings == null)
    throw new InvalidOperationException("JWT settings are not configured in appsettings.json or environment variables.");

builder.Services.AddSingleton<JwtSettings>(jwtSettings);

// Your symmetric secret key (should be stored securely!)
var secretKey = builder.Configuration["Jwt:SecretKey"] ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrWhiteSpace(secretKey))
    throw new InvalidOperationException("JWT secret key is not set in configuration or environment variable.");

var key = Encoding.ASCII.GetBytes(secretKey);

// Configure JWT Bearer authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    // options.Authority = "https://your-auth-server.com"; // Your OAuth 2.1 server
    // options.Audience = "api1"; // Expected Audience in JWT ("aud" claim)
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters        
    {
        ValidateIssuer = false, // set to true and configure as needed
        ValidateAudience = false, // set to true and configure as needed
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key)

    };
    // Optional: Handle events (like token validation failure)    
    // options.Events = ...
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // frontend port
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory Service API", Version = "v1" });
    c.EnableAnnotations();

    //c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    //{
    //    Name = "Authorization",
    //    Type = SecuritySchemeType.Http,
    //    Scheme = "basic",
    //    In = ParameterLocation.Header,
    //    Description = "Basic Authentication using username and password"
    //});

    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
    //{
    //    {
    //        new OpenApiSecurityScheme
    //        {
    //            Reference = new OpenApiReference
    //            {
    //                Type = ReferenceType.SecurityScheme,
    //                Id = "basic"
    //            }
    //        },
    //        new string[] {}
    //    }
    //});

    // Add JWT Bearer token support
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter a valid JWT token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });

});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InventoryDbContext>();
    DbSeeder.Seed(db);
}


// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Inventory Service API is running!").ExcludeFromDescription();

app.Run();

public static class DbSeeder
{
    public static void Seed(InventoryDbContext context)
    {
        if (!context.Products.Any())
        {
            context.Products.AddRange(new List<Product>
            {
                new Product { ProductId = 1, Name = "Pen", Barcode = "PEN001", Price = 10.00m, StockQty = 100, Category = "Stationery", Status = true },
                new Product { ProductId = 2, Name = "Notebook", Barcode = "NOTE001", Price = 50.00m, StockQty = 200, Category = "Stationery", Status = true },
                new Product { ProductId = 3, Name = "Stapler", Barcode = "STPL001", Price = 70.00m, StockQty = 60, Category = "Stationery", Status = true },
                new Product { ProductId = 4, Name = "Eraser", Barcode = "ERSR001", Price = 5.00m, StockQty = 500, Category = "Stationery", Status = true },
                new Product { ProductId = 5, Name = "Backpack", Barcode = "BCKP001", Price = 300.00m, StockQty = 40, Category = "Bags", Status = true },
                new Product { ProductId = 6, Name = "Water Bottle", Barcode = "WBTL001", Price = 120.00m, StockQty = 70, Category = "Accessories", Status = false },
                new Product { ProductId = 7, Name = "Pencil", Barcode = "PNCL001", Price = 8.00m, StockQty = 350, Category = "Stationery", Status = true }
            });
        }

        if(!context.Customers.Any())
        {
            context.Customers.AddRange(new List<Customer>
            {
                new Customer { CustomerId = 1, FullName = "John Doe", Phone = "1234567890", Email = "john@example.com", LoyaltyPoints = 20 },
                new Customer { CustomerId = 2, FullName = "Jane Smith", Phone = "9876543210", Email = "jane@example.com", LoyaltyPoints = 10 },
                new Customer { CustomerId = 3, FullName = "Sam Patel", Phone = "5555555555", Email = "sam.patel@example.com", LoyaltyPoints = 50 },
                new Customer { CustomerId = 4, FullName = "Emily Clark", Phone = "2222222222", Email = "emilyc@example.com", LoyaltyPoints = 0 },
                new Customer { CustomerId = 5, FullName = "Michael Lee", Phone = "3333333333", Email = "mlee@example.com", LoyaltyPoints = 30 },
                new Customer { CustomerId = 6, FullName = "Sara Khan", Phone = "4444444444", Email = "sara.khan@example.com", LoyaltyPoints = 100 }
            });
        }

        if(!context.Sales.Any())
        {
            context.Sales.AddRange(new List<Sale>
            {
                new Sale { SaleId = 1, SaleDate = DateTime.Now.AddDays(-5), CustomerId = 1, TotalAmount = 100.00m, PaidAmount = 100.00m, DueAmount = 0.00m },
                new Sale { SaleId = 2, SaleDate = DateTime.Now.AddDays(-4), CustomerId = 2, TotalAmount = 60.00m, PaidAmount = 60.00m, DueAmount = 0.00m },
                new Sale { SaleId = 3, SaleDate = DateTime.Now.AddDays(-3), CustomerId = 3, TotalAmount = 160.00m, PaidAmount = 100.00m, DueAmount = 60.00m },
                new Sale { SaleId = 4, SaleDate = DateTime.Now.AddDays(-2), CustomerId = 4, TotalAmount = 25.00m, PaidAmount = 25.00m, DueAmount = 0.00m },
                new Sale { SaleId = 5, SaleDate = DateTime.Now.AddDays(-1), CustomerId = 5, TotalAmount = 300.00m, PaidAmount = 150.00m, DueAmount = 150.00m }
            });
        }

        if(!context.SaleDetails.Any())
        {
            context.SaleDetails.AddRange(new List<SaleDetails>
            {
                // Sale 1
                new SaleDetails { SaleDetailId = 1, SaleId = 1, ProductId = 1, Quantity = 5, Price = 10.00m },
                new SaleDetails { SaleDetailId = 2, SaleId = 1, ProductId = 2, Quantity = 1, Price = 50.00m },
                // Sale 2
                new SaleDetails { SaleDetailId = 3, SaleId = 2, ProductId = 4, Quantity = 10, Price = 5.00m },
                new SaleDetails { SaleDetailId = 4, SaleId = 2, ProductId = 3, Quantity = 1, Price = 70.00m },
                // Sale 3
                new SaleDetails { SaleDetailId = 5, SaleId = 3, ProductId = 5, Quantity = 1, Price = 300.00m },
                new SaleDetails { SaleDetailId = 6, SaleId = 3, ProductId = 7, Quantity = 10, Price = 8.00m },
                // Sale 4
                new SaleDetails { SaleDetailId = 7, SaleId = 4, ProductId = 2, Quantity = 1, Price = 50.00m },
                new SaleDetails { SaleDetailId = 8, SaleId = 4, ProductId = 1, Quantity = 3, Price = 10.00m },
                // Sale 5
                new SaleDetails { SaleDetailId = 9, SaleId = 5, ProductId = 5, Quantity = 1, Price = 300.00m }
            });
        }

        if (!context.Users.Any())
        {
            //string password = "UserSuppliedPassword!";
            //string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            context.Users.AddRange(new List<User>
            {
                new User { Id = 1, Username = "admin", Email = "admin@mail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") }
            });
        }

        context.SaveChanges();
    }
}
