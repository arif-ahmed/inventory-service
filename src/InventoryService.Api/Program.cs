using InventoryService.Api.Filters;
using InventoryService.Application;
using InventoryService.Application.Mapping;
using InventoryService.Domain.Entities.Customers;
using InventoryService.Domain.Entities.Products;
using InventoryService.Domain.Entities.Sales;
using InventoryService.Domain.Interfaces;
using InventoryService.Infrastructure.Data;
using InventoryService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddAutoMapper(cfg => {
    cfg.AddProfile<MappingProfile>();
});

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(ApplicationAssemblyReference).Assembly);
});

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseInMemoryDatabase("InventoryDb"));

builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ISalesRepository, SalesRepository>();
builder.Services.AddScoped<ISaleDetailsRepository, SaleDetailsRepository>();

builder.Services.AddSingleton<SalesConcurrencyFilter>();

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

    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authentication using username and password"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
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

            context.Customers.AddRange(new List<Customer>
            {
                new Customer { CustomerId = 1, FullName = "John Doe", Phone = "1234567890", Email = "john@example.com", LoyaltyPoints = 20 },
                new Customer { CustomerId = 2, FullName = "Jane Smith", Phone = "9876543210", Email = "jane@example.com", LoyaltyPoints = 10 },
                new Customer { CustomerId = 3, FullName = "Sam Patel", Phone = "5555555555", Email = "sam.patel@example.com", LoyaltyPoints = 50 },
                new Customer { CustomerId = 4, FullName = "Emily Clark", Phone = "2222222222", Email = "emilyc@example.com", LoyaltyPoints = 0 },
                new Customer { CustomerId = 5, FullName = "Michael Lee", Phone = "3333333333", Email = "mlee@example.com", LoyaltyPoints = 30 },
                new Customer { CustomerId = 6, FullName = "Sara Khan", Phone = "4444444444", Email = "sara.khan@example.com", LoyaltyPoints = 100 }
            });

            context.Sales.AddRange(new List<Sale>
            {
                new Sale { SaleId = 1, SaleDate = DateTime.Now.AddDays(-5), CustomerId = 1, TotalAmount = 100.00m, PaidAmount = 100.00m, DueAmount = 0.00m },
                new Sale { SaleId = 2, SaleDate = DateTime.Now.AddDays(-4), CustomerId = 2, TotalAmount = 60.00m, PaidAmount = 60.00m, DueAmount = 0.00m },
                new Sale { SaleId = 3, SaleDate = DateTime.Now.AddDays(-3), CustomerId = 3, TotalAmount = 160.00m, PaidAmount = 100.00m, DueAmount = 60.00m },
                new Sale { SaleId = 4, SaleDate = DateTime.Now.AddDays(-2), CustomerId = 4, TotalAmount = 25.00m, PaidAmount = 25.00m, DueAmount = 0.00m },
                new Sale { SaleId = 5, SaleDate = DateTime.Now.AddDays(-1), CustomerId = 5, TotalAmount = 300.00m, PaidAmount = 150.00m, DueAmount = 150.00m }
            });

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

            context.SaveChanges();
        }
    }
}
