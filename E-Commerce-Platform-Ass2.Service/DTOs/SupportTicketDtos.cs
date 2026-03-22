using System;
using System.Collections.Generic;

namespace E_Commerce_Platform_Ass2.Service.DTOs
{
    public record CreateTicketRequest(
        string Subject,
        string Description,
        string Category,
        string Priority,
        Guid? RelatedOrderId,
        Guid? RelatedShopId
    );

    public record ReplyTicketRequest(
        Guid TicketId,
        string Content,
        bool IsInternal
    );

    public record UpdateTicketStatusRequest(
        Guid TicketId,
        string Status,
        Guid? AssignedToId,
        string? ResolutionNote
    );

    public record AssignTicketRequest(
        Guid TicketId,
        Guid? AssignedToId
    );

    public record SupportTicketDto
    {
        public Guid Id { get; set; }
        public string TicketCode { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public Guid? RelatedShopId { get; set; }
        public string? RelatedShopName { get; set; }
        public Guid? RelatedOrderId { get; set; }
        public int ResponseCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public DateTime? ClosedAt { get; set; }

        // SLA Fields
        public DateTime? FirstResponseDeadline { get; set; }
        public DateTime? ResolutionDeadline { get; set; }
        public string SlaStatus { get; set; } = "OnTrack";
        public bool FirstResponseMet { get; set; }
        public bool ResolutionMet { get; set; }

        // Escalation
        public int EscalationLevel { get; set; }
        public string? EscalationReason { get; set; }
    }

    public record SupportTicketDetailDto
    {
        public SupportTicketDto Ticket { get; set; } = null!;
        public List<SupportTicketReplyDto> Replies { get; set; } = new();
    }

    public record SupportTicketReplyDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record TicketStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int ResolvedTickets { get; set; }
        public int ClosedTickets { get; set; }
        public int TodayTickets { get; set; }
        public int ThisWeekTickets { get; set; }
        public Dictionary<string, int> ByCategory { get; set; } = new();
        public Dictionary<string, int> ByPriority { get; set; } = new();
        public double AverageResolutionTimeHours { get; set; }
    }

    public record TicketStatisticsExtendedDto : TicketStatisticsDto
    {
        public int SlaAtRiskCount { get; set; }
        public int SlaBreachedCount { get; set; }
        public int UnassignedCount { get; set; }
        public int EscalatedCount { get; set; }
    }

    public record CannedResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatedById { get; set; }
        public string? CreatedByName { get; set; }
    }

    public record CreateCannedResponseRequest(
        string Title,
        string Content,
        string Category,
        string Priority,
        int SortOrder
    );

    public record UpdateCannedResponseRequest(
        Guid Id,
        string Title,
        string Content,
        string Category,
        string Priority,
        bool IsActive,
        int SortOrder
    );

    public record TicketAssignmentRuleDto
    {
        public Guid Id { get; set; }
        public string RuleName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string TicketStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public int PriorityOrder { get; set; }
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public Guid? AssignedToRoleId { get; set; }
        public string? AssignedToRoleName { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public record CreateTicketAssignmentRuleRequest(
        string RuleName,
        string Category,
        string Priority,
        string TicketStatus,
        int PriorityOrder,
        Guid? AssignedToId,
        Guid? AssignedToRoleId
    );

    public record EscalateTicketRequest(
        Guid TicketId,
        int EscalationLevel,
        string Reason
    );
}
