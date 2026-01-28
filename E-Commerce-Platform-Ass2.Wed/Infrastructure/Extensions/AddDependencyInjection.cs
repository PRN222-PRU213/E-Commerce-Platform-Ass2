using E_Commerce_Platform_Ass2.Data.Repositories;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.Services;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions
{
    /// <summary>
    /// Dependency injection extension method to centrally manage all service and repository registrations
    /// </summary>
    public static class AddDependencyInjection
    {
        /// <summary>
        /// Register all services and repositories to the dependency injection container
        /// </summary>
        public static void AddService(this IServiceCollection serviceCollection)
        {
            // Register Repositories
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IRoleRepository, RoleRepository>();
            serviceCollection.AddScoped<IShopRepository, ShopRepository>();
            serviceCollection.AddScoped<IProductRepository, ProductRepository>();
            serviceCollection.AddScoped<IProductVariantRepository, ProductVariantRepostitory>();
            serviceCollection.AddScoped<ICategoryRepository, CategoryRepository>();
            serviceCollection.AddScoped<ICartRepository, CartRepository>();
            serviceCollection.AddScoped<ICartItemRepository, CartItemRepository>();
            serviceCollection.AddScoped<IOrderRepository, OrderRepository>();
            serviceCollection.AddScoped<IOrderItemtRepository, OrderItemRepository>();
            serviceCollection.AddScoped<IPaymentRepository, PaymentRepository>();
            serviceCollection.AddScoped<IReviewRepository, ReviewRepository>();
            serviceCollection.AddScoped<IShipmentRepository, ShipmentRepository>();
            serviceCollection.AddScoped<IRefundRepository, RefundRepository>();
            serviceCollection.AddScoped<IWalletRepository, WalletRepository>();
            serviceCollection.AddScoped<IEKycRepository, EKycRepository>();

            // Register Services
            serviceCollection.AddScoped<IEmailService, EmailService>();
            serviceCollection.AddScoped<IUserService, UserService>();
            serviceCollection.AddScoped<IProductService, ProductService>();
            serviceCollection.AddScoped<ICartService, CartService>();
            serviceCollection.AddScoped<IShopService, ShopService>();
            serviceCollection.AddScoped<IProductVariantService, ProductVariantService>();
            serviceCollection.AddScoped<IAdminService, AdminService>();
            serviceCollection.AddScoped<ICloudinaryService, CloudinaryService>();
            serviceCollection.AddScoped<IShopOrderService, ShopOrderService>();
            serviceCollection.AddScoped<IMomoService, MomoService>();
            serviceCollection.AddScoped<ICheckoutService, CheckoutService>();
            serviceCollection.AddScoped<IOrderService, OrderService>();
            serviceCollection.AddScoped<IRefundService, RefundService>();
            serviceCollection.AddScoped<IWalletService, WalletService>();
            serviceCollection.AddScoped<IEkycService, EKycService>();
            serviceCollection.AddScoped<IVnptEKycService, VnptEKycService>();

            // Register External APIs
            serviceCollection.AddScoped<IMomoApi, MomoApi>();
        }
    }
}
