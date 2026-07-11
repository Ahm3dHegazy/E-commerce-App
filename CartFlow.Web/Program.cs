using CartFlow.Data.Data;
using CartFlow.Services.Interfaces;
using CartFlow.Services.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("constr")));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<ICheckoutService, CheckoutService>();
builder.Services.AddScoped<IAccountService, AccountService>();
// Register Stripe payment service
builder.Services.AddScoped<CartFlow.Services.Interfaces.IPaymentService, CartFlow.Services.Services.StripePaymentService>();
builder.Services.AddScoped<CartFlow.Services.Interfaces.IReviewService, CartFlow.Services.Services.ReviewService>();



builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options => {
        options.LoginPath = "/Account/SignIn";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/SignIn";
    });

// Add session support for anonymous shopping cart storage
builder.Services.AddSession();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        var imagesPath = Path.Combine(
            env.WebRootPath ?? Path.Combine(env.ContentRootPath, "wwwroot"),
            "images", "products");

        await db.Database.MigrateAsync();

        await DbInitializer.SeedAsync(db, imagesPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration/Seeding Error: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Handle non-exception status codes (404, 500, etc.) by re-executing to the Error controller
app.UseStatusCodePagesWithReExecute("/Error/{0}");

app.UseHttpsRedirection();
app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();