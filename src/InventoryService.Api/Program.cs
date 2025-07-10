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
    // Ensure database is created and migrations are applied
    db.Database.Migrate();
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
        if (!context.Users.Any())
        {
            context.Users.AddRange(new List<User>
            {
                new User { Username = "admin", Email = "admin@mail.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123") }
            });
        }

        context.SaveChanges();
    }
}
