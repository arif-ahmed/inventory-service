# 🗃️ Mini Inventory System

## Overview

This is a backend .NET Web API for a basic Inventory System designed to manage Products, Customers, and Sales. The project demonstrates scalable, secure, and maintainable API design following modern .NET development best practices.

---

## ✨ Features 

- **Product Management:** Add, update, delete, and list products 📦
- **Customer Management:** Add, update, delete, and list customers 👥
- **Sales Module:** Create sales transactions, reduce product stock, handle insufficient stock, apply simulated processing delay, and manage global sales concurrency 💸
- **Sales Report:** Get sales summary by date range 📊
- **Authentication:** JWT-based login. All endpoints (except login) are secured 🔐
- **API Tooling:** Swagger UI enabled 🛠️

**Bonus Features:**
- Discount & VAT calculation 🏷️
- Async/await on asynchronous operations ⏳
- Soft delete for Product/Customer 🗑️
- Pagination and filtering on product list endpoint 🔎

---

## ⚙️ Setup Instructions 

### 1. Prerequisites

- [.NET 6.0 SDK or later](https://dotnet.microsoft.com/en-us/download)
- [SQL Server](https://www.microsoft.com/en-gb/sql-server) or [PostgreSQL](https://www.postgresql.org/) (as per your configuration)
- IDE: Visual Studio 2022+, VS Code, or JetBrains Rider

### 2. Database Setup 🛢️

1. Update the connection string in `appsettings.json` to match your database credentials.

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=YOUR_SERVER;Database=YOUR_DATABASE;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
   }

2. Run the SQL script provided in `/db` or `/scripts` folder (if included) to initialise the database schema.

    ```sh
    # Navigate to the src directory
    cd src
    
    # Add initial migration
    dotnet ef migrations add InitialCreate --project InventoryService.Infrastructure --startup-project InventoryService.Api
    
    # Update the database
    dotnet ef database update --project InventoryService.Infrastructure --startup-project InventoryService.Api
    ```


## 🚀 Running the Application

Open a terminal at the project root and run the following commands:

```sh
dotnet restore
dotnet build
dotnet ef database update   # Only if using Entity Framework Core
dotnet run --project src/InventoryService.Api
```

## 🔑 Authentication 

All endpoints (except login) are secured with **JWT authentication**.

**Username:** `admin`  
**Password:** `admin123`

After a successful login, use the returned **JWT Bearer token** for all subsequent API requests.

## API Documentation

Interactive API documentation is available through Swagger.
You can explore the full API, try out endpoints, and see example requests and responses using the interactive Swagger interface.

**Swagger URL:**  
[https://localhost:5001/swagger](https://localhost:5001/swagger)

### How to use Swagger 

1. Open your web browser.
2. Navigate to [https://localhost:5001/swagger](https://localhost:5001/swagger)
3. Browse available endpoints, view details, and interact with the API.
