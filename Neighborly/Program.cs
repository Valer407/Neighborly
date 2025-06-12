using Microsoft.EntityFrameworkCore;
using Neighborly.Data;
using Neighborly.Models.DBModels;
using Neighborly.Models;
using System.Text.Json;
using System.IO;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

// Ensure database and seed categories from JSON if needed
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
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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