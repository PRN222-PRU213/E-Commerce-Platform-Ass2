using System.Security.Claims;
using E_Commerce_Platform_Ass2.Data.Database;
using E_Commerce_Platform_Ass2.Data.Momo;
using E_Commerce_Platform_Ass2.Service.Common.Configurations;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.BackgroundJobs;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Configure MomoAPI
builder.Services.Configure<MomoConfig>(builder.Configuration.GetSection("MomoAPI"));

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR(options =>
{
    // Show full exception details in development so client-side errors are useful.
    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors = true;
    }
});

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
builder.Services.Configure<VnptEKycConfig>(builder.Configuration.GetSection("VnptEKyc"));

// Configure RefundBusinessRules
builder.Services.Configure<E_Commerce_Platform_Ass2.Service.Options.RefundBusinessRules>(
    builder.Configuration.GetSection(
        E_Commerce_Platform_Ass2.Service.Options.RefundBusinessRules.SectionName
    )
);

// Register all repositories & services via extension method
builder.Services.AddService();
builder.Services.AddHostedService<AiChatFallbackWorker>();

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
app.MapRazorPages().WithStaticAssets();

app.MapHub<E_Commerce_Platform_Ass2.Wed.Hubs.NotificationHub>("/hubs/notifications");
app.MapHub<ChatHub>("/hubs/chat");

// ── AI Personal Shopper Minimal API endpoints ─────────────────────────────────
// Registered as Minimal API so DisableAntiforgery() works reliably without
// requiring an app restart (Razor Pages [IgnoreAntiforgeryToken] needs restart).
app.MapPost(
        "/Api/PersonalShopper/Chat",
        async (
            HttpRequest httpRequest,
            IPersonalShopperService shopperService,
            ILoggerFactory loggerFactory
        ) =>
        {
            var logger = loggerFactory.CreateLogger("PersonalShopper.Chat");
            ShopperChatRequest? req = null;
            try
            {
                req = await httpRequest.ReadFromJsonAsync<ShopperChatRequest>();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to deserialize PersonalShopper chat request");
            }

            if (req == null || string.IsNullOrWhiteSpace(req.Message))
                return Results.Json(new { error = "Message is required." }, statusCode: 400);

            try
            {
                var response = await shopperService.ChatAsync(req.Message, req.History ?? new());
                return Results.Json(response);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PersonalShopper ChatAsync failed");
                return Results.Json(
                    new ShopperChatResponse
                    {
                        Message =
                            $"\u26a0\ufe0f Xin l\u1ed7i, t\u00f4i \u0111ang g\u1eb7p s\u1ef1 c\u1ed1: {ex.Message}",
                        Combos = null,
                    }
                );
            }
        }
    )
    .DisableAntiforgery();

app.MapPost(
        "/Api/PersonalShopper/AddCombo",
        async (
            HttpRequest httpRequest,
            ClaimsPrincipal user,
            IPersonalShopperService shopperService,
            ICartService cartService,
            IHubContext<NotificationHub> hubContext,
            ILoggerFactory loggerFactory
        ) =>
        {
            var logger = loggerFactory.CreateLogger("PersonalShopper.AddCombo");
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            AddComboToCartRequest? req = null;
            try
            {
                req = await httpRequest.ReadFromJsonAsync<AddComboToCartRequest>();
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Failed to deserialize AddCombo request");
            }

            if (req == null || req.VariantIds == null || req.VariantIds.Count == 0)
                return Results.Json(new { error = "VariantIds is required." }, statusCode: 400);

            try
            {
                var result = await shopperService.AddComboToCartAsync(userId, req.VariantIds);
                if (result.AddedCount == 0)
                {
                    return Results.Json(
                        new
                        {
                            success = false,
                            error = result.Message,
                            result,
                        },
                        statusCode: 400
                    );
                }

                var cartItemCount = await cartService.GetTotalItemCountAsync(userId);
                await hubContext
                    .Clients.Group($"user-{userId}")
                    .SendAsync("CartCountUpdated", cartItemCount);

                return Results.Json(
                    new
                    {
                        success = true,
                        count = result.AddedCount,
                        cartItemCount,
                        result,
                    }
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "PersonalShopper AddCombo failed");
                return Results.StatusCode(500);
            }
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

// ── Pre-order Minimal API endpoints ───────────────────────────────────────────
app.MapPost(
        "/Api/PreOrder/Create",
        async (
            ClaimsPrincipal user,
            HttpRequest httpRequest,
            IPreOrderService preOrderService,
            IHubContext<NotificationHub> hubContext
        ) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            var req = await httpRequest.ReadFromJsonAsync<CreatePreOrderRequest>();
            if (req == null)
                return Results.Json(new { error = "Invalid request body." }, statusCode: 400);

            try
            {
                var result = await preOrderService.CreateAsync(userId, req);
                await hubContext
                    .Clients.Group($"user-{userId}")
                    .SendAsync("PreOrderCreated", result);
                await hubContext
                    .Clients.Group($"shop-{result.ShopId}")
                    .SendAsync("PreOrderCreated", result);
                return Results.Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Results.Json(new { success = false, error = ex.Message }, statusCode: 400);
            }
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

app.MapPost(
        "/Api/PreOrder/PayDeposit",
        async (
            ClaimsPrincipal user,
            HttpRequest httpRequest,
            IPreOrderService preOrderService,
            IHubContext<NotificationHub> hubContext
        ) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            var req = await httpRequest.ReadFromJsonAsync<PayPreOrderDepositRequest>();
            if (req == null)
                return Results.Json(new { error = "Invalid request body." }, statusCode: 400);

            try
            {
                var result = await preOrderService.PayDepositAsync(userId, req);
                await hubContext
                    .Clients.Group($"user-{userId}")
                    .SendAsync("PreOrderDepositPaid", result);
                await hubContext
                    .Clients.Group($"shop-{result.ShopId}")
                    .SendAsync("PreOrderDepositPaid", result);
                return Results.Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Results.Json(new { success = false, error = ex.Message }, statusCode: 400);
            }
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

app.MapPost(
        "/Api/PreOrder/MarkReadyForFinalPayment",
        async (
            ClaimsPrincipal user,
            HttpRequest httpRequest,
            IPreOrderService preOrderService,
            IHubContext<NotificationHub> hubContext
        ) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            var req = await httpRequest.ReadFromJsonAsync<MarkPreOrderReadyRequest>();
            if (req == null)
                return Results.Json(new { error = "Invalid request body." }, statusCode: 400);

            try
            {
                var result = await preOrderService.MarkReadyForFinalPaymentAsync(userId, req);
                await hubContext
                    .Clients.Group($"shop-{result.ShopId}")
                    .SendAsync("PreOrderReadyForFinalPayment", result);
                await hubContext
                    .Clients.Group($"user-{result.UserId}")
                    .SendAsync("PreOrderReadyForFinalPayment", result);
                return Results.Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Results.Json(new { success = false, error = ex.Message }, statusCode: 400);
            }
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

app.MapPost(
        "/Api/PreOrder/PayRemaining",
        async (
            ClaimsPrincipal user,
            HttpRequest httpRequest,
            IPreOrderService preOrderService,
            IHubContext<NotificationHub> hubContext
        ) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            var req = await httpRequest.ReadFromJsonAsync<FinalizePreOrderPaymentRequest>();
            if (req == null)
                return Results.Json(new { error = "Invalid request body." }, statusCode: 400);

            try
            {
                var result = await preOrderService.PayRemainingAsync(userId, req);
                await hubContext
                    .Clients.Group($"user-{result.UserId}")
                    .SendAsync("PreOrderPaymentCompleted", result);
                await hubContext
                    .Clients.Group($"shop-{result.ShopId}")
                    .SendAsync("PreOrderPaymentCompleted", result);
                return Results.Json(new { success = true, result });
            }
            catch (Exception ex)
            {
                return Results.Json(new { success = false, error = ex.Message }, statusCode: 400);
            }
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

app.MapGet(
        "/Api/PreOrder/MyOrders",
        async (ClaimsPrincipal user, IPreOrderService preOrderService) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            var items = await preOrderService.GetMyPreOrdersAsync(userId);
            return Results.Json(items);
        }
    )
    .RequireAuthorization();

app.MapGet(
        "/Api/Shop/PreOrders",
        async (ClaimsPrincipal user, IPreOrderService preOrderService) =>
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId))
                return Results.Unauthorized();

            try
            {
                var items = await preOrderService.GetShopPreOrdersAsync(userId);
                return Results.Json(items);
            }
            catch (Exception ex)
            {
                return Results.Json(new { error = ex.Message }, statusCode: 400);
            }
        }
    )
    .RequireAuthorization();

app.MapPost(
        "/Api/PreOrder/ExpireOverdue",
        async (ClaimsPrincipal user, IPreOrderService preOrderService) =>
        {
            var role = user.FindFirstValue(ClaimTypes.Role);
            if (!string.Equals(role, "Admin", StringComparison.OrdinalIgnoreCase))
                return Results.Forbid();

            var count = await preOrderService.ExpireOverduePreOrdersAsync();
            return Results.Json(new { success = true, expired = count });
        }
    )
    .DisableAntiforgery()
    .RequireAuthorization();

// ─────────────────────────────────────────────────────────────────────────────

app.Run();
