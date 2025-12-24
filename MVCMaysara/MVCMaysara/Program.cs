using Microsoft.EntityFrameworkCore;
using MVCMaysara.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Add DbContext with SQL Server
builder.Services.AddDbContext<MaysaraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MaysaraConnection")));

// 2. Configure Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.Name = ".MaysaraMVC.Session";
});

// 3. Add MVC Controllers with Views
builder.Services.AddControllersWithViews();

// 4. Add API controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null; // PascalCase
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession(); // BEFORE UseAuthorization
app.UseAuthorization();

// Map routes
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");
app.MapControllers(); // For API routes

app.Run();
