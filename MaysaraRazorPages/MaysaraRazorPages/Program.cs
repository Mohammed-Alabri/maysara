using Microsoft.EntityFrameworkCore;
using MaysaraRazorPages.Data;
using MaysaraRazorPages.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure Entity Framework DbContext with SQL Server
builder.Services.AddDbContext<MaysaraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MaysaraConnection")));

// Register ADO.NET Data Access service
builder.Services.AddScoped<AdoNetDataAccess>();

// Configure Session for state management
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add Razor Pages with Authorization Filter
builder.Services.AddRazorPages()
    .AddMvcOptions(options =>
    {
        options.Filters.Add<AuthorizePageFilter>();
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

// Enable session before authorization
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
