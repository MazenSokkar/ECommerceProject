using ecommerce.Core.Entities;
using ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ecommerce.Infrastructure.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Categories
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "Electronics", Slug = "electronics", Active = true },
                new() { Name = "Fashion", Slug = "fashion", Active = true },
                new() { Name = "Home", Slug = "home", Active = true },
                new() { Name = "Books", Slug = "books", Active = true },
            };
            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();

            // Subcategories
            var electronics = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "electronics");
            var subCategories = new List<Category>
            {
                new() { Name = "Phones", Slug = "phones", ParentId = electronics!.Id, Active = true },
                new() { Name = "Laptops", Slug = "laptops", ParentId = electronics!.Id, Active = true },
            };
            await context.Categories.AddRangeAsync(subCategories);
            await context.SaveChangesAsync();
        }

        // Products
        if (!await context.Products.AnyAsync())
        {
            var merchant = await context.Merchants.FirstOrDefaultAsync();
            if (merchant == null) return;

            var phones = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "phones");
            var laptops = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "laptops");
            var fashion = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "fashion");
            var home = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "home");
            var books = await context.Categories.FirstOrDefaultAsync(c => c.Slug == "books");

            if (phones == null || laptops == null || fashion == null || home == null || books == null) return;

            var products = new List<Product>
            {
                // Phones
                new() { Name = "iPhone 15 Pro", Description = "Latest Apple iPhone with A17 Pro chip", Price = 999.99m, Stock = 50, CategoryId = phones.Id, MerchantId = merchant.Id, IsActive = true, IsFeatured = true },
                new() { Name = "Samsung Galaxy S24", Description = "Samsung flagship with AI features", Price = 849.99m, Stock = 30, CategoryId = phones.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Google Pixel 8 Pro", Description = "Google's best camera phone", Price = 799.99m, Stock = 25, CategoryId = phones.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "OnePlus 12", Description = "Flagship killer with fast charging", Price = 699.99m, Stock = 40, CategoryId = phones.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Xiaomi 14 Ultra", Description = "Leica camera system phone", Price = 749.99m, Stock = 20, CategoryId = phones.Id, MerchantId = merchant.Id, IsActive = true },

                // Laptops
                new() { Name = "MacBook Pro M3", Description = "Apple MacBook Pro with M3 chip", Price = 1999.99m, Stock = 20, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true, IsFeatured = true },
                new() { Name = "Dell XPS 15", Description = "Premium Windows laptop with OLED display", Price = 1799.99m, Stock = 15, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "HP Spectre x360", Description = "2-in-1 convertible laptop", Price = 1499.99m, Stock = 18, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Lenovo ThinkPad X1 Carbon", Description = "Business ultrabook", Price = 1599.99m, Stock = 12, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "ASUS ROG Zephyrus G14", Description = "Gaming laptop with AMD Ryzen", Price = 1399.99m, Stock = 10, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Microsoft Surface Pro 9", Description = "Tablet and laptop in one", Price = 1299.99m, Stock = 22, CategoryId = laptops.Id, MerchantId = merchant.Id, IsActive = true },

                // Fashion
                new() { Name = "Nike Air Max 270", Description = "Comfortable everyday sneakers", Price = 149.99m, Stock = 100, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true, IsFeatured = true },
                new() { Name = "Adidas Ultraboost 23", Description = "Running shoes with Boost technology", Price = 179.99m, Stock = 80, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Levi's 501 Jeans", Description = "Classic straight fit jeans", Price = 89.99m, Stock = 150, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Zara Leather Jacket", Description = "Premium faux leather jacket", Price = 129.99m, Stock = 60, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "H&M Casual Shirt", Description = "Comfortable everyday shirt", Price = 39.99m, Stock = 200, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Ray-Ban Aviator Sunglasses", Description = "Classic aviator style", Price = 199.99m, Stock = 45, CategoryId = fashion.Id, MerchantId = merchant.Id, IsActive = true },

                // Home
                new() { Name = "Dyson V15 Vacuum", Description = "Cordless vacuum with laser detection", Price = 749.99m, Stock = 25, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true, IsFeatured = true },
                new() { Name = "Nespresso Vertuo Pop", Description = "Coffee machine with 5 cup sizes", Price = 149.99m, Stock = 40, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "IKEA BILLY Bookcase", Description = "Classic adjustable bookcase", Price = 79.99m, Stock = 60, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Philips Hue Starter Kit", Description = "Smart lighting system", Price = 199.99m, Stock = 35, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "KitchenAid Stand Mixer", Description = "Professional 5Qt stand mixer", Price = 449.99m, Stock = 20, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Roomba i7 Robot Vacuum", Description = "Smart robot vacuum with mapping", Price = 599.99m, Stock = 15, CategoryId = home.Id, MerchantId = merchant.Id, IsActive = true },

                // Books
                new() { Name = "Clean Code", Description = "A handbook of agile software craftsmanship", Price = 39.99m, Stock = 100, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true, IsFeatured = true },
                new() { Name = "The Pragmatic Programmer", Description = "Your journey to mastery", Price = 49.99m, Stock = 80, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Design Patterns", Description = "Elements of reusable object-oriented software", Price = 54.99m, Stock = 70, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Atomic Habits", Description = "An easy way to build good habits", Price = 24.99m, Stock = 200, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "The Lean Startup", Description = "How modern companies grow", Price = 29.99m, Stock = 150, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true },
                new() { Name = "Deep Work", Description = "Rules for focused success", Price = 27.99m, Stock = 120, CategoryId = books.Id, MerchantId = merchant.Id, IsActive = true },
            };

            await context.Products.AddRangeAsync(products);
            await context.SaveChangesAsync();
        }
    }
}