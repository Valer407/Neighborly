using Microsoft.EntityFrameworkCore;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using System.Text.Json;
using System.IO;
using System.Globalization;
var builder = WebApplication.CreateBuilder(args);


var culture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;


builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddDbContext<NeighborlyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<NeighborlyContext>();
    context.Database.Migrate();

    if (!context.Categories.Any())
    {
        var jsonFile = Path.Combine(app.Environment.WebRootPath, "data", "categories.json");
        if (File.Exists(jsonFile))
        {
            var jsonData = File.ReadAllText(jsonFile);
            var categories = JsonSerializer.Deserialize<List<Categories>>(jsonData);
            if (categories != null)
            {
                foreach (var cat in categories)
                {
                    cat.CategoryId = 0;
                    cat.IconSvg = Icons.GetIcon(cat.Icon);
                }
                context.Categories.AddRange(categories);
                context.SaveChanges();
            }
        }
    }

    if (!context.Listing_Types.Any())
    {
        var jsonFile = Path.Combine(app.Environment.WebRootPath, "data", "listing_types.json");
        if (File.Exists(jsonFile))
        {
            var jsonData = File.ReadAllText(jsonFile);
            var types = JsonSerializer.Deserialize<List<Listing_types>>(jsonData);
            if (types != null)
            {
                foreach (var lt in types)
                {
                    lt.ListingTypeId = 0;
                }
                context.Listing_Types.AddRange(types);
                context.SaveChanges();
            }
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();