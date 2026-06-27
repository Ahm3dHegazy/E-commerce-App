using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public static class DbInitializer {
    public static async Task SeedAsync(AppDbContext db) {
        if (await db.Categories.AnyAsync()) return;

        var catApparel = new Category { Name = "Apparel", Descripion = "Clothing and fashion wearables" };
        var catAccessories = new Category { Name = "Accessories", Descripion = "Small add-ons and complements" };
        var catElectronics = new Category { Name = "Electronics", Descripion = "Gadgets and electronic devices" };

        var subHoodies = new Category { Name = "Hoodies", Descripion = "Warm hooded sweatshirts", ParentCategory = catApparel };
        var subTshirts = new Category { Name = "T-Shirts", Descripion = "Casual cotton t-shirts", ParentCategory = catApparel };
        var subJeans = new Category { Name = "Jeans", Descripion = "Denim pants and jackets", ParentCategory = catApparel };

        var subMugs = new Category { Name = "Mugs", Descripion = "Ceramic drinkware", ParentCategory = catAccessories };
        var subCaps = new Category { Name = "Caps", Descripion = "Adjustable baseball caps", ParentCategory = catAccessories };
        var subBags = new Category { Name = "Bags", Descripion = "Backpacks and totes", ParentCategory = catAccessories };

        var subHeadphones = new Category { Name = "Headphones", Descripion = "Wired and wireless audio", ParentCategory = catElectronics };
        var subChargers = new Category { Name = "Chargers", Descripion = "Wall chargers and cables", ParentCategory = catElectronics };

        catApparel.Subcategories.AddRange([subHoodies, subTshirts, subJeans]);
        catAccessories.Subcategories.AddRange([subMugs, subCaps, subBags]);
        catElectronics.Subcategories.AddRange([subHeadphones, subChargers]);

        var products = new List<Product> {
            new() { Name = "Classic Hoodie", Description = "Comfortable cotton-blend hoodie with front pouch pocket.", StockQuantity = 25, UnitPrice = 599m, Category = subHoodies },
            new() { Name = "Zip-Up Hoodie", Description = "Lightweight zip-up hoodie with adjustable drawstring hood.", StockQuantity = 18, UnitPrice = 749m, Category = subHoodies },
            new() { Name = "Slim Fit Tee", Description = "Soft ringspun cotton t-shirt with a modern slim fit.", StockQuantity = 40, UnitPrice = 249m, Category = subTshirts },
            new() { Name = "Graphic Print Tee", Description = "Bold graphic print on premium heavyweight cotton.", StockQuantity = 30, UnitPrice = 299m, Category = subTshirts },
            new() { Name = "Slim Denim Jeans", Description = "Classic five-pocket slim jeans in dark wash.", StockQuantity = 22, UnitPrice = 899m, Category = subJeans },
            new() { Name = "Denim Jacket", Description = "Classic denim jacket with button front and chest pockets.", StockQuantity = 12, UnitPrice = 1299m, Category = subJeans },
            new() { Name = "Ceramic Coffee Mug", Description = "Glossy white ceramic mug, 350ml capacity.", StockQuantity = 50, UnitPrice = 149m, Category = subMugs },
            new() { Name = "Enamel Mug", Description = "Vintage-style speckled enamel mug, perfect for camping.", StockQuantity = 35, UnitPrice = 179m, Category = subMugs },
            new() { Name = "Cotton Baseball Cap", Description = "Adjustable snapback cap in solid colors.", StockQuantity = 45, UnitPrice = 199m, Category = subCaps },
            new() { Name = "Trucker Cap", Description = "Breathable mesh-back trucker cap with foam front.", StockQuantity = 28, UnitPrice = 229m, Category = subCaps },
            new() { Name = "Canvas Backpack", Description = "Durable canvas backpack with padded laptop sleeve.", StockQuantity = 20, UnitPrice = 549m, Category = subBags },
            new() { Name = "Tote Bag", Description = "Minimalist cotton tote bag, perfect for groceries.", StockQuantity = 60, UnitPrice = 129m, Category = subBags },
            new() { Name = "Wireless Headphones", Description = "Over-ear Bluetooth headphones with 30h battery.", StockQuantity = 15, UnitPrice = 1299m, Category = subHeadphones },
            new() { Name = "In-Ear Earbuds", Description = "Lightweight wired earbuds with built-in microphone.", StockQuantity = 40, UnitPrice = 349m, Category = subHeadphones },
            new() { Name = "Fast Wall Charger", Description = "65W GaN USB-C fast charger with dual ports.", StockQuantity = 33, UnitPrice = 449m, Category = subChargers },
            new() { Name = "USB-C Cable 2M", Description = "Braided nylon USB-C to USB-C cable, 2 meters.", StockQuantity = 80, UnitPrice = 99m, Category = subChargers },
        };

        var user = new User {
            FirstName = "Ahmed",
            LastName = "Ali",
            Email = "ahmed@example.com",
            Password = "123456",
            UserRole = Role.CUSTOMER,
            Phone = "01001234567"
        };

        db.Categories.Add(catApparel);
        db.Categories.Add(catAccessories);
        db.Categories.Add(catElectronics);
        db.AddRange(subHoodies, subTshirts, subJeans, subMugs, subCaps, subBags, subHeadphones, subChargers);
        db.Products.AddRange(products);
        db.Users.Add(user);

        await db.SaveChangesAsync();
    }
}
