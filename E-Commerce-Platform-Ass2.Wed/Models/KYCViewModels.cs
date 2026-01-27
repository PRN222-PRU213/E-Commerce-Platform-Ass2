using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace E_Commerce_Platform_Ass2.Wed.Models
{
    public class VerifyViewModel
    {
        [Required(ErrorMessage = "Vui lòng tải lên ảnh mặt trước CCCD")]
        [Display(Name = "Ảnh mặt trước CCCD")]
        public IFormFile FrontCard { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng tải lên ảnh mặt sau CCCD")]
        [Display(Name = "Ảnh mặt sau CCCD")]
        public IFormFile BackCard { get; set; } = default!;

        [Required(ErrorMessage = "Vui lòng chụp ảnh chân dung")]
        [Display(Name = "Ảnh chân dung")]
        public IFormFile Selfie { get; set; } = default!;
    }

    public class KYCStatusViewModel
    {
        public bool IsVerified { get; set; }
        public string Message { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string CccdNumber { get; set; } = string.Empty;
        public double FaceMatchScore { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}

