using Microsoft.EntityFrameworkCore;
using Peracraft.Data;
using Peracraft.Services;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

// Test HashPassword
string TestHashPassword(string sifre)
{
    if (string.IsNullOrEmpty(sifre)) return string.Empty;

    using (var md5 = MD5.Create())
    {
        var inputBytes = Encoding.UTF8.GetBytes(sifre);
        var hashBytes = md5.ComputeHash(inputBytes);
        var sb = new StringBuilder();
        
        for (int i = 0; i < hashBytes.Length; i++)
        {
            sb.Append(hashBytes[i].ToString("x2"));
        }
        
        return sb.ToString();
    }
}

Console.WriteLine("Test şifre hash'i: " + TestHashPassword("123456"));

// Add services to the container.
builder.Services.AddControllersWithViews();

try 
{
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseMySql(
            builder.Configuration.GetConnectionString("DefaultConnection"),
            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))
        ));
}
catch (Exception ex)
{
    Console.WriteLine($"Veritabanı bağlantı hatası: {ex.Message}");
    throw;
}

// Session ekle
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();

// Servis kayıtları
builder.Services.AddScoped<IUrunService, UrunService>();

var app = builder.Build();

// Hata yakalama middleware'i
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot")),
    RequestPath = ""
});

app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

try
{
    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Uygulama çalışma hatası: {ex.Message}");
    throw;
}
