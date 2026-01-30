using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Momo;
using E_Commerce_Platform_Ass2.Service.Common.Configurations;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure MomoAPI
builder.Services.Configure<MomoConfig>(builder.Configuration.GetSection("MomoAPI"));

// Add services to the container.
builder.Services.AddRazorPages();

// Sessions
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Authentication
builder
    .Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
    });

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Configure Cloudinary
builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("CloudinarySettings")
);

// Configure VNPT eKYC (optional - required if you enable KYC feature)
builder.Services.Configure<VnptEKycConfig>(
    builder.Configuration.GetSection("VnptEKyc")
);

// Configure RefundBusinessRules
builder.Services.Configure<E_Commerce_Platform_Ass2.Service.Options.RefundBusinessRules>(
    builder.Configuration.GetSection(E_Commerce_Platform_Ass2.Service.Options.RefundBusinessRules.SectionName)
);

// Register all repositories & services via extension method
builder.Services.AddService();

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

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
