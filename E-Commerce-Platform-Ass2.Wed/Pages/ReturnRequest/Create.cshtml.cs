using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.ReturnRequest
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly IReturnRequestService _returnRequestService;
        private readonly IOrderService _orderService;
        private readonly ICloudinaryService _cloudinaryService;

        public CreateModel(
            IReturnRequestService returnRequestService,
            IOrderService orderService,
            ICloudinaryService cloudinaryService)
        {
            _returnRequestService = returnRequestService;
            _orderService = orderService;
            _cloudinaryService = cloudinaryService;
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
                RequestType = Input.RequestType,
                Reason = Input.Reason,
                ReasonDetail = Input.ReasonDetail,
                EvidenceImageUrls = imageUrls,
                RequestedAmount = Input.RequestedAmount
            };

            var result = await _returnRequestService.CreateRequestAsync(dto);

            if (result.IsSuccess)
            {
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
