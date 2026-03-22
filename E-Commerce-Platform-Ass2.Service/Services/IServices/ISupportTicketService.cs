using E_Commerce_Platform_Ass2.Service.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.Services.IServices
{
    public interface ISupportTicketService
    {
        Task<SupportTicketDto> CreateTicketAsync(Guid customerId, CreateTicketRequest request);
        Task<SupportTicketDetailDto?> GetTicketByIdAsync(Guid ticketId);
        Task<SupportTicketDetailDto?> GetTicketByCodeAsync(string ticketCode);
        Task<List<SupportTicketDto>> GetMyTicketsAsync(Guid customerId);
        Task<List<SupportTicketDto>> GetTicketsForShopAsync(Guid shopId);
        Task<List<SupportTicketDto>> GetTicketsForAdminAsync(string? status, string? priority, string? category);
        Task<SupportTicketDto> AssignTicketAsync(Guid ticketId, Guid adminId, Guid? assignedToId);
        Task<SupportTicketDto> UpdateTicketStatusAsync(Guid ticketId, Guid userId, string userRole, UpdateTicketStatusRequest request);
        Task<SupportTicketReplyDto> AddReplyAsync(Guid ticketId, Guid senderId, string senderRole, ReplyTicketRequest request);
        Task<List<SupportTicketReplyDto>> GetRepliesAsync(Guid ticketId);
        Task<bool> CloseTicketAsync(Guid ticketId, Guid userId, string userRole);
        Task<TicketStatisticsDto> GetStatisticsAsync();
        Task<TicketStatisticsExtendedDto> GetStatisticsExtendedAsync();

        // SLA Methods
        Task<SupportTicketDto> EscalateTicketAsync(Guid ticketId, Guid userId, EscalateTicketRequest request);
        Task<List<SupportTicketDto>> GetSlaAtRiskAsync();
        Task<List<SupportTicketDto>> GetSlaBreachedAsync();
        Task<List<SupportTicketDto>> GetUnassignedAsync();
        Task CheckAndUpdateSlaStatusAsync();

        // Canned Responses
        Task<List<CannedResponseDto>> GetCannedResponsesAsync(string? category = null);
        Task<CannedResponseDto> CreateCannedResponseAsync(Guid createdById, CreateCannedResponseRequest request);
        Task<CannedResponseDto> UpdateCannedResponseAsync(Guid userId, UpdateCannedResponseRequest request);
        Task<bool> DeleteCannedResponseAsync(Guid id);

        // Assignment Rules
        Task<List<TicketAssignmentRuleDto>> GetAssignmentRulesAsync();
        Task<TicketAssignmentRuleDto> CreateAssignmentRuleAsync(Guid createdById, CreateTicketAssignmentRuleRequest request);
        Task<TicketAssignmentRuleDto> UpdateAssignmentRuleAsync(Guid ruleId, CreateTicketAssignmentRuleRequest request);
        Task<bool> DeleteAssignmentRuleAsync(Guid id);
    }
}
