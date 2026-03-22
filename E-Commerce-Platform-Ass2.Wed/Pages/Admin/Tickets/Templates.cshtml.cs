using System.Security.Claims;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace E_Commerce_Platform_Ass2.Wed.Pages.Admin.Tickets
{
    [Authorize(Roles = "Admin")]
    public class TemplatesModel : PageModel
    {
        private readonly ISupportTicketService _ticketService;

        public TemplatesModel(ISupportTicketService ticketService)
        {
            _ticketService = ticketService;
        }

        public List<CannedResponseDto> CannedResponses { get; set; } = new();
        public List<TicketAssignmentRuleDto> AssignmentRules { get; set; } = new();
        public List<AuthenticatedUser> AllAdmins { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? FilterCategory { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            CannedResponses = await _ticketService.GetCannedResponsesAsync(FilterCategory);
            AssignmentRules = await _ticketService.GetAssignmentRulesAsync();
            return Page();
        }

        public async Task<IActionResult> OnPostCreateTemplateAsync()
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var title = Request.Form["Title"].ToString();
            var content = Request.Form["Content"].ToString();
            var category = Request.Form["Category"].ToString();
            var priority = Request.Form["Priority"].ToString();
            var sortOrder = int.TryParse(Request.Form["SortOrder"].ToString(), out var so) ? so : 0;

            var request = new CreateCannedResponseRequest(
                Title: title,
                Content: content,
                Category: category,
                Priority: priority,
                SortOrder: sortOrder
            );

            await _ticketService.CreateCannedResponseAsync(adminId, request);
            TempData["Success"] = "Template đã được tạo thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteTemplateAsync(Guid id)
        {
            await _ticketService.DeleteCannedResponseAsync(id);
            TempData["Success"] = "Template đã được xóa!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCreateRuleAsync()
        {
            var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var ruleName = Request.Form["RuleName"].ToString();
            var category = Request.Form["RuleCategory"].ToString();
            var priority = Request.Form["RulePriority"].ToString();
            var priorityOrder = int.TryParse(Request.Form["RulePriorityOrder"].ToString(), out var po) ? po : 0;
            var assignedToIdStr = Request.Form["AssignedToId"].ToString();
            var assignedToRoleIdStr = Request.Form["AssignedToRoleId"].ToString();

            Guid? assignedToId = string.IsNullOrEmpty(assignedToIdStr) ? null : Guid.Parse(assignedToIdStr);
            Guid? assignedToRoleId = string.IsNullOrEmpty(assignedToRoleIdStr) ? null : Guid.Parse(assignedToRoleIdStr);

            var request = new CreateTicketAssignmentRuleRequest(
                RuleName: ruleName,
                Category: category,
                Priority: priority,
                TicketStatus: "Open",
                PriorityOrder: priorityOrder,
                AssignedToId: assignedToId,
                AssignedToRoleId: assignedToRoleId
            );

            await _ticketService.CreateAssignmentRuleAsync(adminId, request);
            TempData["Success"] = "Rule đã được tạo thành công!";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteRuleAsync(Guid id)
        {
            await _ticketService.DeleteAssignmentRuleAsync(id);
            TempData["Success"] = "Rule đã được xóa!";
            return RedirectToPage();
        }
    }
}
