using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Hubs;
using E_Commerce_Platform_Ass2.Wed.Models;
using E_Commerce_Platform_Ass2.Wed.Models.SignalR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;

namespace E_Commerce_Platform_Ass2.Wed.Pages.ReturnRequest
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderService _orderService;
        private readonly ICloudinaryService _cloudinaryService;
        private readonly IShopService _shopService;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<NotificationHub, INotificationClient> _hubContext;

        public CreateModel(
            IReturnRequestService returnRequestService,
            IOrderService orderService,
            ICloudinaryService cloudinaryService,
            IShopService shopService,
            INotificationService notificationService,
            IHubContext<NotificationHub, INotificationClient> hubContext)
        {
            _returnRequestService = returnRequestService;
            _orderService = orderService;
            _cloudinaryService = cloudinaryService;
            _shopService = shopService;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [BindProperty(SupportsGet = true)]
        public Guid OrderId { get; set; }

        [BindProperty]
        public CreateReturnRequestViewModel Input { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Check if can create request
            var canCreate = await _returnRequestService.CanCreateRequestAsync(OrderId, userId);
            if (!canCreate)
            {
                TempData["Error"] = "Không thể tạo yêu cầu hoàn trả cho đơn hàng này.";
                return RedirectToPage("/Order/History");
            }

            var order = await _orderService.GetOrderItemAsync(OrderId);
            if (order == null) return NotFound();

            Input = new CreateReturnRequestViewModel
            {
                OrderId = OrderId,
                RequestType = "Return",
                OrderDate = order.CreatedAt,
                TotalAmount = order.TotalAmount,
                RequestedAmount = order.TotalAmount,
                Items = order.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Size = i.Size,
                    Color = i.Color
                }).ToList()
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(List<IFormFile>? evidenceImages)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            if (evidenceImages == null || !evidenceImages.Any(f => f.Length > 0))
            {
                ModelState.AddModelError(string.Empty, "Vui lòng tải lên ít nhất 1 hình ảnh bằng chứng để shop xác nhận.");
            }

            if (!ModelState.IsValid)
            {
                // Reload order items
                var order = await _orderService.GetOrderItemAsync(Input.OrderId);
                if (order != null)
                {
                    Input.Items = order.Items.Select(i => new OrderItemViewModel
                    {
                        ProductName = i.ProductName,
                        ImageUrl = i.ImageUrl ?? string.Empty,
                        Price = i.Price,
                        Quantity = i.Quantity,
                        Size = i.Size,
                        Color = i.Color
                    }).ToList();
                    Input.TotalAmount = order.TotalAmount;
                    Input.OrderDate = order.CreatedAt;
                }
                return Page();
            }

            // Upload evidence images
            var imageUrls = new List<string>();
            if (evidenceImages != null && evidenceImages.Any())
            {
                foreach (var image in evidenceImages.Take(5))
                {
                    if (image.Length > 0)
                    {
                        var imageUrl = await _cloudinaryService.UploadImageAsync(image, "return-requests");
                        if (!string.IsNullOrEmpty(imageUrl))
                            imageUrls.Add(imageUrl);
                    }
                }
            }

            var dto = new CreateReturnRequestDto
            {
                OrderId = Input.OrderId,
                UserId = userId,
                RequestType = "Return",
                Reason = Input.Reason,
                ReasonDetail = Input.ReasonDetail,
                EvidenceImageUrls = imageUrls,
                RequestedAmount = Input.RequestedAmount
            };

            var result = await _returnRequestService.CreateRequestAsync(dto);

            if (result.IsSuccess)
            {
                // Notify related shops + admins about new return request
                try
                {
                    var shopIds = await _orderService.GetShopIdsByOrderAsync(Input.OrderId);
                    foreach (var shopId in shopIds)
                    {
                        var shop = await _shopService.GetShopByIdAsync(shopId);
                        if (shop == null)
                        {
                            continue;
                        }

                        var shopMessage =
                            $"Khách hàng đã gửi yêu cầu hoàn hàng cho đơn #{Input.OrderId.ToString()[..8].ToUpper()}";
                        var shopLink = $"/Shop/Orders/ReturnRequestDetail?id={result.Data?.Id}";

                        await _notificationService.CreateNotificationAsync(shop.UserId, "warning", shopMessage, shopLink);

                        await _hubContext.Clients.Group($"shop-{shopId}").NotificationReceived(new NotificationMessage
                        {
                            Type = "warning",
                            Message = shopMessage,
                            Link = shopLink,
                            UserId = userId
                        });
                    }

                    var notification = new NotificationMessage
                    {
                        Type = "warning",
                        Message = $"Yêu cầu hoàn trả mới cho đơn hàng #{Input.OrderId.ToString()[..8].ToUpper()}",
                        Link = "/Shop/Orders/ReturnRequests",
                        UserId = userId
                    };
                    await _hubContext.Clients.Group("admins").NotificationReceived(notification);
                }
                catch
                {
                    // Don't let notification failure break the return request flow
                }

                TempData["Success"] = "Yêu cầu hoàn trả đã được gửi thành công! Vui lòng chờ xét duyệt.";
                return RedirectToPage("/ReturnRequest/Index");
            }

            TempData["Error"] = result.ErrorMessage;

            // Reload order for view
            var orderReload = await _orderService.GetOrderItemAsync(Input.OrderId);
            if (orderReload != null)
            {
                Input.Items = orderReload.Items.Select(i => new OrderItemViewModel
                {
                    ProductName = i.ProductName,
                    ImageUrl = i.ImageUrl ?? string.Empty,
                    Price = i.Price,
                    Quantity = i.Quantity,
                    Size = i.Size,
                    Color = i.Color
                }).ToList();
                Input.TotalAmount = orderReload.TotalAmount;
                Input.OrderDate = orderReload.CreatedAt;
            }

            return Page();
        }
    }
}
