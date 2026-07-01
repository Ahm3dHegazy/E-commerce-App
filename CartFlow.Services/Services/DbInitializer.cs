using CartFlow.Data.Data;
using CartFlow.Data.Entities;
using CartFlow.Data.Enums;
using Microsoft.EntityFrameworkCore;

namespace CartFlow.Services.Services;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext db)
    {
        var catApparel = await GetOrCreateCategoryAsync(db, "Apparel", "Clothing and fashion wearables", null);
        var catAccessories = await GetOrCreateCategoryAsync(db, "Accessories", "Small add-ons and complements", null);
        var catElectronics = await GetOrCreateCategoryAsync(db, "Electronics", "Gadgets and electronic devices", null);

        // Apparel subcategories (4 existing + 4 new)
        var subHoodies = await GetOrCreateCategoryAsync(db, "Hoodies", "Warm hooded sweatshirts", catApparel);
        var subTshirts = await GetOrCreateCategoryAsync(db, "T-Shirts", "Casual cotton t-shirts", catApparel);
        var subJeans = await GetOrCreateCategoryAsync(db, "Jeans", "Denim pants and jackets", catApparel);
        var subShorts = await GetOrCreateCategoryAsync(db, "Shorts", "Casual and athletic shorts", catApparel);
        var subSweaters = await GetOrCreateCategoryAsync(db, "Sweaters", "Knitted pullovers and cardigans", catApparel);
        var subJackets = await GetOrCreateCategoryAsync(db, "Jackets", "Outerwear for all seasons", catApparel);
        var subSportswear = await GetOrCreateCategoryAsync(db, "Sportswear", "Activewear and gym clothing", catApparel);

        // Accessories subcategories (3 existing + 4 new)
        var subMugs = await GetOrCreateCategoryAsync(db, "Mugs", "Ceramic drinkware", catAccessories);
        var subCaps = await GetOrCreateCategoryAsync(db, "Caps", "Adjustable baseball caps", catAccessories);
        var subBags = await GetOrCreateCategoryAsync(db, "Bags", "Backpacks and totes", catAccessories);
        var subWatches = await GetOrCreateCategoryAsync(db, "Watches", "Analog and digital timepieces", catAccessories);
        var subSunglasses = await GetOrCreateCategoryAsync(db, "Sunglasses", "UV-protective eyewear", catAccessories);
        var subWallets = await GetOrCreateCategoryAsync(db, "Wallets", "Compact card holders and bifolds", catAccessories);
        var subJewelry = await GetOrCreateCategoryAsync(db, "Jewelry", "Necklaces, bracelets and more", catAccessories);

        // Electronics subcategories (2 existing + 4 new)
        var subHeadphones = await GetOrCreateCategoryAsync(db, "Headphones", "Wired and wireless audio", catElectronics);
        var subChargers = await GetOrCreateCategoryAsync(db, "Chargers", "Wall chargers and cables", catElectronics);
        var subSmartwatches = await GetOrCreateCategoryAsync(db, "Smartwatches", "Wearable tech and fitness trackers", catElectronics);
        var subSpeakers = await GetOrCreateCategoryAsync(db, "Speakers", "Portable and smart speakers", catElectronics);
        var subKeyboards = await GetOrCreateCategoryAsync(db, "Keyboards", "Mechanical and wireless keyboards", catElectronics);
        var subMice = await GetOrCreateCategoryAsync(db, "Mice", "Wired and wireless computer mice", catElectronics);

        await db.SaveChangesAsync();

        // Apparel products (6 existing + 8 new)
        await GetOrCreateProductAsync(db, "Classic Hoodie", "Comfortable cotton-blend hoodie with front pouch pocket.", 25, 599m, subHoodies);
        await GetOrCreateProductAsync(db, "Zip-Up Hoodie", "Lightweight zip-up hoodie with adjustable drawstring hood.", 18, 749m, subHoodies);
        await GetOrCreateProductAsync(db, "Slim Fit Tee", "Soft ringspun cotton t-shirt with a modern slim fit.", 40, 249m, subTshirts);
        await GetOrCreateProductAsync(db, "Graphic Print Tee", "Bold graphic print on premium heavyweight cotton.", 30, 299m, subTshirts);
        await GetOrCreateProductAsync(db, "Slim Denim Jeans", "Classic five-pocket slim jeans in dark wash.", 22, 899m, subJeans);
        await GetOrCreateProductAsync(db, "Denim Jacket", "Classic denim jacket with button front and chest pockets.", 12, 1299m, subJeans);
        await GetOrCreateProductAsync(db, "Athletic Shorts", "Breathable moisture-wicking shorts for training.", 35, 349m, subShorts);
        await GetOrCreateProductAsync(db, "Chino Shorts", "Tailored cotton chino shorts in neutral colors.", 20, 399m, subShorts);
        await GetOrCreateProductAsync(db, "Cashmere Sweater", "Luxuriously soft cashmere crewneck sweater.", 10, 1499m, subSweaters);
        await GetOrCreateProductAsync(db, "Knit Cardigan", "Open-front knit cardigan with ribbed cuffs.", 15, 899m, subSweaters);
        await GetOrCreateProductAsync(db, "Bomber Jacket", "Classic satin bomber jacket with ribbed trim.", 14, 1299m, subJackets);
        await GetOrCreateProductAsync(db, "Rain Jacket", "Waterproof lightweight rain jacket with hood.", 18, 849m, subJackets);
        await GetOrCreateProductAsync(db, "Jogger Pants", "Elastic cuff joggers with adjustable drawstring waist.", 30, 499m, subSportswear);
        await GetOrCreateProductAsync(db, "Performance Tee", "Quick-dry performance top for intense workouts.", 50, 299m, subSportswear);

        // Accessories products (6 existing + 8 new)
        await GetOrCreateProductAsync(db, "Ceramic Coffee Mug", "Glossy white ceramic mug, 350ml capacity.", 50, 149m, subMugs);
        await GetOrCreateProductAsync(db, "Enamel Mug", "Vintage-style speckled enamel mug, perfect for camping.", 35, 179m, subMugs);
        await GetOrCreateProductAsync(db, "Cotton Baseball Cap", "Adjustable snapback cap in solid colors.", 45, 199m, subCaps);
        await GetOrCreateProductAsync(db, "Trucker Cap", "Breathable mesh-back trucker cap with foam front.", 28, 229m, subCaps);
        await GetOrCreateProductAsync(db, "Canvas Backpack", "Durable canvas backpack with padded laptop sleeve.", 20, 549m, subBags);
        await GetOrCreateProductAsync(db, "Tote Bag", "Minimalist cotton tote bag, perfect for groceries.", 60, 129m, subBags);
        await GetOrCreateProductAsync(db, "Analog Watch", "Classic analog watch with leather strap and mineral glass.", 15, 799m, subWatches);
        await GetOrCreateProductAsync(db, "Digital Sports Watch", "Water-resistant digital watch with stopwatch and alarm.", 25, 449m, subWatches);
        await GetOrCreateProductAsync(db, "Aviator Sunglasses", "Timeless aviator style with UV400 protection.", 20, 599m, subSunglasses);
        await GetOrCreateProductAsync(db, "Wayfarer Sunglasses", "Iconic wayfarer frame with polarized lenses.", 22, 499m, subSunglasses);
        await GetOrCreateProductAsync(db, "Leather Bifold Wallet", "Genuine leather bifold wallet with RFID blocking.", 18, 349m, subWallets);
        await GetOrCreateProductAsync(db, "Minimalist Card Holder", "Slim aluminum card holder for up to 4 cards.", 40, 199m, subWallets);
        await GetOrCreateProductAsync(db, "Silver Chain Necklace", "Sterling silver cable chain necklace, 50cm.", 25, 449m, subJewelry);
        await GetOrCreateProductAsync(db, "Beaded Bracelet", "Handcrafted lava stone beaded bracelet with adjustable cord.", 35, 179m, subJewelry);

        // Electronics products (4 existing + 8 new)
        await GetOrCreateProductAsync(db, "Wireless Headphones", "Over-ear Bluetooth headphones with 30h battery.", 15, 1299m, subHeadphones);
        await GetOrCreateProductAsync(db, "In-Ear Earbuds", "Lightweight wired earbuds with built-in microphone.", 40, 349m, subHeadphones);
        await GetOrCreateProductAsync(db, "Fast Wall Charger", "65W GaN USB-C fast charger with dual ports.", 33, 449m, subChargers);
        await GetOrCreateProductAsync(db, "USB-C Cable 2M", "Braided nylon USB-C to USB-C cable, 2 meters.", 80, 99m, subChargers);
        await GetOrCreateProductAsync(db, "Fitness Smartwatch", "Water-resistant fitness tracker with heart rate monitor.", 12, 1999m, subSmartwatches);
        await GetOrCreateProductAsync(db, "Hybrid Smartwatch", "Classic analog look with smart notifications.", 8, 2499m, subSmartwatches);
        await GetOrCreateProductAsync(db, "Bluetooth Speaker", "Compact portable speaker with 12h playtime.", 20, 699m, subSpeakers);
        await GetOrCreateProductAsync(db, "Smart Speaker", "Voice-controlled smart speaker with room-filling sound.", 10, 999m, subSpeakers);
        await GetOrCreateProductAsync(db, "Mechanical Keyboard", "RGB mechanical keyboard with blue switches.", 18, 799m, subKeyboards);
        await GetOrCreateProductAsync(db, "Wireless Slim Keyboard", "Ultra-slim rechargeable keyboard for desk and travel.", 30, 449m, subKeyboards);
        await GetOrCreateProductAsync(db, "Wireless Mouse", "Ergonomic silent-click wireless mouse.", 40, 349m, subMice);
        await GetOrCreateProductAsync(db, "Gaming Mouse", "High-DPI gaming mouse with customizable buttons.", 15, 599m, subMice);

        await db.SaveChangesAsync();

        // Seed user
        if (!await db.Users.AnyAsync(u => u.Email == "ahmed@example.com"))
        {
            db.Users.Add(new User
            {
                FirstName = "Ahmed",
                LastName = "Ali",
                Email = "ahmed@example.com",
                Password = "123456",
                UserRole = Role.CUSTOMER,
                Phone = "01001234567"
            });
            await db.SaveChangesAsync();
        }
    }

    private static async Task<Category> GetOrCreateCategoryAsync(AppDbContext db, string name, string description, Category? parent)
    {
        var existing = await db.Categories.FirstOrDefaultAsync(c => c.Name == name);
        if (existing is not null) return existing;

        var category = new Category { Name = name, Descripion = description, ParentCategory = parent };
        db.Categories.Add(category);
        await db.SaveChangesAsync();
        return category;
    }

    private static async Task GetOrCreateProductAsync(AppDbContext db, string name, string description, int stock, decimal price, Category category)
    {
        if (await db.Products.AnyAsync(p => p.Name == name)) return;

        db.Products.Add(new Product
        {
            Name = name,
            Description = description,
            StockQuantity = stock,
            UnitPrice = price,
            CategoryId = category.Id
        });
        await db.SaveChangesAsync();
    }
}