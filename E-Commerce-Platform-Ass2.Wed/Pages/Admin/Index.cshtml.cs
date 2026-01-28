using E_Commerce_Platform_Ass2.Service.Services.IServices;
using E_Commerce_Platform_Ass2.Wed.Infrastructure.Extensions;
using E_Commerce_Platform_Ass2.Wed.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly IAdminService _adminService;

        public IndexModel(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public AdminDashboardViewModel Dashboard { get; set; } = new();

        public async Task OnGetAsync()
        {
            var dashboardDto = await _adminService.GetDashboardStatisticsAsync();
            Dashboard = dashboardDto.ToViewModel();
        }
    }
}

