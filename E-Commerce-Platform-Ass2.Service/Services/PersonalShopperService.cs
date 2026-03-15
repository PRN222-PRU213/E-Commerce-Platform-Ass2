using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class PersonalShopperService : IPersonalShopperService
    {
        private readonly IGeminiService _geminiService;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;
        private readonly IProductVariantRepository _productVariantRepository;

        public PersonalShopperService(
            IGeminiService geminiService,
            IProductService productService,
            ICartService cartService,
            IProductVariantRepository productVariantRepository
        )
        {
            _geminiService = geminiService;
            _productService = productService;
            _cartService = cartService;
            _productVariantRepository = productVariantRepository;
        }

        public async Task<ShopperChatResponse> ChatAsync(
            string userMessage,
            List<ShopperMessageDto> history
        )
        {
            var products = await _productService.GetActiveProductsWithVariantsAsync();
            var catalog = BuildProductCatalog(products);
            var prompt = BuildPrompt(catalog, history, userMessage);

            string rawResponse;
            try
            {
                rawResponse = await _geminiService.GenerateContentAsync(prompt);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Gemini API thất bại: {ex.Message}", ex);
            }

            return ParseResponse(rawResponse, products);
        }

        public async Task<AddComboToCartResult> AddComboToCartAsync(
            Guid userId,
            List<Guid> variantIds
        )
        {
            var result = new AddComboToCartResult { RequestedCount = variantIds?.Count ?? 0 };

            if (variantIds == null || variantIds.Count == 0)
            {
                result.Message = "Khong co san pham nao de them vao gio.";
                return result;
            }

            // Cap so luong item tranh spam payload qua lon.
            var normalizedVariantIds = variantIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .Take(20)
                .ToList();

            foreach (var variantId in normalizedVariantIds)
            {
                var variant = await _productVariantRepository.GetByIdAsync(variantId);
                if (variant == null)
                {
                    result.SkippedInvalidCount++;
                    result.SkippedVariantIds.Add(variantId);
                    continue;
                }

                if (!string.Equals(variant.Status, "Active", StringComparison.OrdinalIgnoreCase))
                {
                    result.SkippedInactiveCount++;
                    result.SkippedVariantIds.Add(variantId);
                    continue;
                }

                if (variant.Stock <= 0)
                {
                    result.SkippedOutOfStockCount++;
                    result.SkippedVariantIds.Add(variantId);
                    continue;
                }

                await _cartService.AddToCart(userId, variantId, 1);
                result.AddedCount++;
                result.AddedVariantIds.Add(variantId);
            }

            result.Message =
                result.AddedCount > 0
                    ? $"Da them {result.AddedCount} san pham vao gio hang."
                    : "Khong co san pham hop le de them vao gio.";

            return result;
        }

        private static string BuildProductCatalog(List<ProductDetailDto> products)
        {
            var sb = new StringBuilder();
            foreach (var p in products.Take(80))
            {
                var activeVariants = p.Variants.Where(v => v.Stock > 0).Take(5).ToList();
                if (!activeVariants.Any())
                    continue;

                sb.AppendLine(
                    $"[PID:{p.Id}] {p.Name} | Cat:{p.CategoryName} | Shop:{p.ShopName} | BasePrice:{p.BasePrice:F0}"
                );
                foreach (var v in activeVariants)
                {
                    sb.AppendLine(
                        $"  [VID:{v.Id}] {v.VariantName} | Size:{v.Size ?? "N/A"} | Color:{v.Color ?? "N/A"} | Price:{v.Price:F0} | Stock:{v.Stock}"
                    );
                }
            }
            return sb.ToString();
        }

        private static string BuildPrompt(
            string catalog,
            List<ShopperMessageDto> history,
            string userMessage
        )
        {
            var sb = new StringBuilder();
            sb.AppendLine(
                "Bạn là AI Personal Shopper của nền tảng thương mại điện tử. Nhiệm vụ của bạn là giúp người dùng tìm và mua các bộ sản phẩm hoàn chỉnh phù hợp với nhu cầu."
            );
            sb.AppendLine();
            sb.AppendLine("QUY TRÌNH:");
            sb.AppendLine(
                "1. Hỏi ngắn gọn để hiểu mục đích, ngân sách, phong cách, size nếu chưa rõ."
            );
            sb.AppendLine(
                "2. Dựa trên catalog sản phẩm để tạo 2-4 combo phù hợp, mỗi combo gồm nhiều sản phẩm."
            );
            sb.AppendLine(
                "3. Khi sẵn sàng đề xuất, LUÔN thêm khối <COMBOS>...</COMBOS> ở CUỐI phản hồi."
            );
            sb.AppendLine(
                "4. Nếu chưa đủ thông tin, hỏi thêm (không đề xuất combo khi chưa rõ nhu cầu)."
            );
            sb.AppendLine("5. Giải thích ngắn gọn lý do chọn mỗi combo.");
            sb.AppendLine();
            sb.AppendLine("ĐỊNH DẠNG <COMBOS> BẮT BUỘC (JSON array, không kèm markdown):");
            sb.AppendLine(
                "<COMBOS>[{\"name\":\"Tên bộ\",\"style\":\"formal\",\"description\":\"Lý do\",\"products\":[{\"productId\":\"GUID-here\",\"variantId\":\"GUID-here\",\"name\":\"Tên SP\",\"price\":100000,\"variantName\":\"Tên variant\",\"imageUrl\":null}]}]</COMBOS>"
            );
            sb.AppendLine();
            sb.AppendLine(
                "LƯU Ý: Chỉ dùng productId và variantId từ catalog bên dưới. Không bịa GUID."
            );
            sb.AppendLine();
            sb.AppendLine("=== CATALOG SẢN PHẨM ===");
            sb.AppendLine(catalog);
            sb.AppendLine("=== KẾT THÚC CATALOG ===");
            sb.AppendLine();
            sb.AppendLine("=== LỊCH SỬ HỘI THOẠI ===");
            foreach (var msg in history.TakeLast(12))
            {
                var role = msg.Role == "user" ? "Người dùng" : "AI Shopper";
                sb.AppendLine($"{role}: {msg.Content}");
            }
            sb.AppendLine();
            sb.AppendLine($"Người dùng: {userMessage}");
            sb.AppendLine("AI Shopper:");
            return sb.ToString();
        }

        private static ShopperChatResponse ParseResponse(
            string rawResponse,
            List<ProductDetailDto> products
        )
        {
            var combosMatch = Regex.Match(
                rawResponse,
                @"<COMBOS>(.*?)</COMBOS>",
                RegexOptions.Singleline
            );
            var textMessage = rawResponse;
            List<ProductComboDto>? combos = null;

            if (combosMatch.Success)
            {
                textMessage = rawResponse.Replace(combosMatch.Value, string.Empty).Trim();
                try
                {
                    var jsonStr = combosMatch.Groups[1].Value.Trim();
                    var parsed = JsonSerializer.Deserialize<List<ComboJsonDto>>(
                        jsonStr,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );

                    if (parsed != null)
                    {
                        combos = parsed
                            .Select(c => new ProductComboDto
                            {
                                Name = c.Name,
                                Style = c.Style,
                                Description = c.Description,
                                Products = c
                                    .Products.Select(p =>
                                    {
                                        var product = products.FirstOrDefault(x =>
                                            x.Id == p.ProductId
                                        );
                                        var variant = product?.Variants.FirstOrDefault(v =>
                                            v.Id == p.VariantId
                                        );
                                        return new PersonalShopperProductDto
                                        {
                                            ProductId = p.ProductId,
                                            VariantId = p.VariantId,
                                            Name = string.IsNullOrWhiteSpace(p.Name)
                                                ? (product?.Name ?? "Sản phẩm")
                                                : p.Name,
                                            Price = variant?.Price ?? p.Price,
                                            VariantName = p.VariantName ?? variant?.VariantName,
                                            ImageUrl =
                                                variant?.ImageUrl
                                                ?? product?.ImageUrl
                                                ?? p.ImageUrl,
                                            Color = variant?.Color,
                                            Size = variant?.Size,
                                        };
                                    })
                                    .ToList(),
                                TotalPrice = c.Products.Sum(p =>
                                {
                                    var product = products.FirstOrDefault(x => x.Id == p.ProductId);
                                    var variant = product?.Variants.FirstOrDefault(v =>
                                        v.Id == p.VariantId
                                    );
                                    return variant?.Price ?? p.Price;
                                }),
                            })
                            .ToList();
                    }
                }
                catch
                {
                    // Ignore parse errors; return text message only
                }
            }

            return new ShopperChatResponse { Message = textMessage, Combos = combos };
        }

        private sealed class ComboJsonDto
        {
            public string Name { get; set; } = string.Empty;
            public string Style { get; set; } = string.Empty;
            public string Description { get; set; } = string.Empty;
            public List<ComboProductJsonDto> Products { get; set; } = new();
        }

        private sealed class ComboProductJsonDto
        {
            public Guid ProductId { get; set; }
            public Guid VariantId { get; set; }
            public string Name { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string? VariantName { get; set; }
            public string? ImageUrl { get; set; }
        }
    }
}
