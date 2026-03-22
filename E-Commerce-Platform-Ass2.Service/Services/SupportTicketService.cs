using E_Commerce_Platform_Ass2.Data.Database.Entities;
using E_Commerce_Platform_Ass2.Data.Repositories.Interfaces;
using E_Commerce_Platform_Ass2.Service.DTOs;
using E_Commerce_Platform_Ass2.Service.Services.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace E_Commerce_Platform_Ass2.Service.Services
{
    public class SupportTicketService : ISupportTicketService
    {
        private readonly ISupportTicketRepository _ticketRepository;
        private readonly ISupportTicketReplyRepository _replyRepository;
        private readonly IUserRepository _userRepository;
        private readonly ICannedResponseRepository _cannedResponseRepository;
        private readonly ITicketAssignmentRuleRepository _assignmentRuleRepository;

        // SLA Time thresholds (in hours)
        private static readonly Dictionary<string, (int FirstResponseHours, int ResolutionHours)> SlaTimeConfig = new()
        {
            { "Urgent", (1, 4) },
            { "High", (4, 24) },
            { "Medium", (8, 48) },
            { "Low", (24, 72) }
        };

        public SupportTicketService(
            ISupportTicketRepository ticketRepository,
            ISupportTicketReplyRepository replyRepository,
            IUserRepository userRepository,
            ICannedResponseRepository cannedResponseRepository,
            ITicketAssignmentRuleRepository assignmentRuleRepository
        )
        {
            _ticketRepository = ticketRepository;
            _replyRepository = replyRepository;
            _userRepository = userRepository;
            _cannedResponseRepository = cannedResponseRepository;
            _assignmentRuleRepository = assignmentRuleRepository;
        }

        public async Task<SupportTicketDto> CreateTicketAsync(Guid customerId, CreateTicketRequest request)
        {
            var customer = await _userRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }

            var ticketCode = await _ticketRepository.GenerateTicketCodeAsync();

            // Calculate SLA deadlines based on priority
            var slaConfig = SlaTimeConfig.GetValueOrDefault(request.Priority, (8, 48));
            var now = DateTime.UtcNow;
            var firstResponseHours = slaConfig.Item1;
            var resolutionHours = slaConfig.Item2;

            var ticket = new SupportTicket
            {
                CustomerId = customerId,
                TicketCode = ticketCode,
                Subject = request.Subject,
                Description = request.Description,
                Category = request.Category,
                Priority = request.Priority,
                RelatedOrderId = request.RelatedOrderId,
                RelatedShopId = request.RelatedShopId,
                Status = "Open",
                CreatedAt = now,
                FirstResponseDeadline = now.AddHours(firstResponseHours),
                ResolutionDeadline = now.AddHours(resolutionHours),
                SlaStatus = "OnTrack",
                EscalationLevel = 0,
                FirstResponseMet = false,
                ResolutionMet = false
            };

            var created = await _ticketRepository.CreateAsync(ticket);

            // Auto-assign based on rules
            await TryAutoAssignAsync(created);

            return MapToDto(created, customer);
        }

        private async Task TryAutoAssignAsync(SupportTicket ticket)
        {
            var matchingRule = await _assignmentRuleRepository.GetMatchingRuleAsync(ticket.Category, ticket.Priority);
            if (matchingRule != null)
            {
                if (matchingRule.AssignedToId.HasValue)
                {
                    ticket.AssignedToId = matchingRule.AssignedToId;
                    ticket.Status = "InProgress";
                    ticket.UpdatedAt = DateTime.UtcNow;
                    await _ticketRepository.UpdateAsync(ticket);
                }
                else if (matchingRule.AssignedToRoleId.HasValue)
                {
                    // Find first active user with that role
                    var users = await _userRepository.GetAllAsync();
                    var eligibleUser = users.FirstOrDefault(u =>
                        u.Role?.RoleId == matchingRule.AssignedToRoleId && u.Status);
                    if (eligibleUser != null)
                    {
                        ticket.AssignedToId = eligibleUser.Id;
                        ticket.Status = "InProgress";
                        ticket.UpdatedAt = DateTime.UtcNow;
                        await _ticketRepository.UpdateAsync(ticket);
                    }
                }
            }
        }

        public async Task<SupportTicketDetailDto?> GetTicketByIdAsync(Guid ticketId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket == null) return null;

            var replies = await _replyRepository.GetByTicketIdAsync(ticketId);
            var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);

            var replyDtos = replies.Select(r => new SupportTicketReplyDto
            {
                Id = r.Id,
                TicketId = r.TicketId,
                SenderId = r.SenderId,
                SenderName = r.Sender?.Name ?? "Unknown",
                SenderRole = r.SenderRole,
                Content = r.Content,
                IsInternal = r.IsInternal,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new SupportTicketDetailDto
            {
                Ticket = MapToDto(ticket, customer ?? ticket.Customer),
                Replies = replyDtos
            };
        }

        public async Task<SupportTicketDetailDto?> GetTicketByCodeAsync(string ticketCode)
        {
            var ticket = await _ticketRepository.GetByTicketCodeAsync(ticketCode);
            if (ticket == null) return null;

            var replies = await _replyRepository.GetByTicketIdAsync(ticket.Id);
            var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);

            var replyDtos = replies.Select(r => new SupportTicketReplyDto
            {
                Id = r.Id,
                TicketId = r.TicketId,
                SenderId = r.SenderId,
                SenderName = r.Sender?.Name ?? "Unknown",
                SenderRole = r.SenderRole,
                Content = r.Content,
                IsInternal = r.IsInternal,
                CreatedAt = r.CreatedAt
            }).ToList();

            return new SupportTicketDetailDto
            {
                Ticket = MapToDto(ticket, customer ?? ticket.Customer),
                Replies = replyDtos
            };
        }

        public async Task<List<SupportTicketDto>> GetMyTicketsAsync(Guid customerId)
        {
            var tickets = await _ticketRepository.GetByCustomerIdAsync(customerId);
            var customer = await _userRepository.GetByIdAsync(customerId);
            return tickets.Select(t => MapToDto(t, customer)).ToList();
        }

        public async Task<List<SupportTicketDto>> GetTicketsForShopAsync(Guid shopId)
        {
            var tickets = await _ticketRepository.GetByShopIdAsync(shopId);
            var result = new List<SupportTicketDto>();

            foreach (var ticket in tickets)
            {
                var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
                result.Add(MapToDto(ticket, customer));
            }

            return result;
        }

        public async Task<List<SupportTicketDto>> GetTicketsForAdminAsync(string? status, string? priority, string? category)
        {
            var tickets = await _ticketRepository.GetAllAsync(status, priority, category);
            var result = new List<SupportTicketDto>();

            foreach (var ticket in tickets)
            {
                var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
                result.Add(MapToDto(ticket, customer));
            }

            return result;
        }

        public async Task<SupportTicketDto> AssignTicketAsync(Guid ticketId, Guid adminId, Guid? assignedToId)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Không tìm thấy ticket.");
            }

            ticket.AssignedToId = assignedToId;
            if (ticket.Status == "Open")
            {
                ticket.Status = "InProgress";
            }
            ticket.UpdatedAt = DateTime.UtcNow;

            var updated = await _ticketRepository.UpdateAsync(ticket);
            var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
            return MapToDto(updated, customer ?? ticket.Customer);
        }

        public async Task<SupportTicketDto> UpdateTicketStatusAsync(
            Guid ticketId,
            Guid userId,
            string userRole,
            UpdateTicketStatusRequest request)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Không tìm thấy ticket.");
            }

            ticket.Status = request.Status;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (request.Status == "Resolved")
            {
                ticket.ResolvedAt = DateTime.UtcNow;
                // Check if resolution SLA was met
                if (ticket.ResolutionDeadline.HasValue && DateTime.UtcNow <= ticket.ResolutionDeadline.Value)
                {
                    ticket.ResolutionMet = true;
                }
            }
            else if (request.Status == "Closed")
            {
                ticket.ClosedAt = DateTime.UtcNow;
                if (!ticket.ResolvedAt.HasValue)
                {
                    ticket.ResolvedAt = DateTime.UtcNow;
                }
                if (ticket.ResolutionDeadline.HasValue && DateTime.UtcNow <= ticket.ResolutionDeadline.Value)
                {
                    ticket.ResolutionMet = true;
                }
            }

            if (request.AssignedToId.HasValue)
            {
                ticket.AssignedToId = request.AssignedToId.Value;
            }

            var updated = await _ticketRepository.UpdateAsync(ticket);

            if (!string.IsNullOrEmpty(request.ResolutionNote))
            {
                var reply = new SupportTicketReply
                {
                    TicketId = ticketId,
                    SenderId = userId,
                    SenderRole = userRole,
                    Content = request.ResolutionNote,
                    IsInternal = false,
                    CreatedAt = DateTime.UtcNow
                };
                await _replyRepository.CreateAsync(reply);
            }

            var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
            return MapToDto(updated, customer ?? ticket.Customer);
        }

        public async Task<SupportTicketReplyDto> AddReplyAsync(
            Guid ticketId,
            Guid senderId,
            string senderRole,
            ReplyTicketRequest request)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Không tìm thấy ticket.");
            }

            // Mark first response if this is the first non-internal reply
            if (!ticket.FirstResponseMet && senderRole != "Customer")
            {
                ticket.FirstResponseAt = DateTime.UtcNow;
                ticket.FirstResponseMet = true;
                if (ticket.FirstResponseDeadline.HasValue &&
                    DateTime.UtcNow <= ticket.FirstResponseDeadline.Value)
                {
                    ticket.FirstResponseMet = true;
                }
                else
                {
                    ticket.FirstResponseMet = ticket.FirstResponseDeadline.HasValue &&
                                              DateTime.UtcNow <= ticket.FirstResponseDeadline.Value;
                }
            }

            var reply = new SupportTicketReply
            {
                TicketId = ticketId,
                SenderId = senderId,
                SenderRole = senderRole,
                Content = request.Content,
                IsInternal = request.IsInternal && (senderRole == "Admin" || senderRole == "Shop"),
                CreatedAt = DateTime.UtcNow
            };

            var created = await _replyRepository.CreateAsync(reply);

            ticket.ResponseCount = await _replyRepository.GetReplyCountByTicketIdAsync(ticketId);
            ticket.UpdatedAt = DateTime.UtcNow;
            await _ticketRepository.UpdateAsync(ticket);

            var sender = await _userRepository.GetByIdAsync(senderId);

            return new SupportTicketReplyDto
            {
                Id = created.Id,
                TicketId = created.TicketId,
                SenderId = created.SenderId,
                SenderName = sender?.Name ?? "Unknown",
                SenderRole = created.SenderRole,
                Content = created.Content,
                IsInternal = created.IsInternal,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<List<SupportTicketReplyDto>> GetRepliesAsync(Guid ticketId)
        {
            var replies = await _replyRepository.GetByTicketIdAsync(ticketId);
            return replies.Select(r => new SupportTicketReplyDto
            {
                Id = r.Id,
                TicketId = r.TicketId,
                SenderId = r.SenderId,
                SenderName = r.Sender?.Name ?? "Unknown",
                SenderRole = r.SenderRole,
                Content = r.Content,
                IsInternal = r.IsInternal,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<bool> CloseTicketAsync(Guid ticketId, Guid userId, string userRole)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null) return false;

            if (ticket.Status == "Closed")
            {
                return true;
            }

            ticket.Status = "Closed";
            ticket.ClosedAt = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            if (!ticket.ResolvedAt.HasValue)
            {
                ticket.ResolvedAt = DateTime.UtcNow;
            }

            if (ticket.ResolutionDeadline.HasValue && DateTime.UtcNow <= ticket.ResolutionDeadline.Value)
            {
                ticket.ResolutionMet = true;
            }

            await _ticketRepository.UpdateAsync(ticket);

            var reply = new SupportTicketReply
            {
                TicketId = ticketId,
                SenderId = userId,
                SenderRole = userRole,
                Content = "Ticket đã được đóng.",
                IsInternal = false,
                CreatedAt = DateTime.UtcNow
            };
            await _replyRepository.CreateAsync(reply);

            return true;
        }

        public async Task<TicketStatisticsDto> GetStatisticsAsync()
        {
            var open = await _ticketRepository.GetCountByStatusAsync("Open");
            var inProgress = await _ticketRepository.GetCountByStatusAsync("InProgress");
            var pending = await _ticketRepository.GetCountByStatusAsync("Pending");
            var resolved = await _ticketRepository.GetCountByStatusAsync("Resolved");
            var closed = await _ticketRepository.GetCountByStatusAsync("Closed");
            var today = await _ticketRepository.GetTodayCountAsync();
            var thisWeek = await _ticketRepository.GetThisWeekCountAsync();
            var avgResolution = await _ticketRepository.GetAverageResolutionTimeAsync();
            var byCategory = await _ticketRepository.GetCountByCategoryAsync();
            var byPriority = await _ticketRepository.GetCountByPriorityAsync();

            var total = open + inProgress + pending + resolved + closed;

            return new TicketStatisticsDto
            {
                TotalTickets = total,
                OpenTickets = open,
                InProgressTickets = inProgress,
                ResolvedTickets = resolved,
                ClosedTickets = closed,
                TodayTickets = today,
                ThisWeekTickets = thisWeek,
                ByCategory = byCategory,
                ByPriority = byPriority,
                AverageResolutionTimeHours = Math.Round(avgResolution, 1)
            };
        }

        public async Task<TicketStatisticsExtendedDto> GetStatisticsExtendedAsync()
        {
            var basic = await GetStatisticsAsync();
            var slaAtRisk = await _ticketRepository.GetSlaAtRiskAsync();
            var slaBreached = await _ticketRepository.GetSlaBreachedAsync();
            var unassigned = await _ticketRepository.GetUnassignedAsync();

            return new TicketStatisticsExtendedDto
            {
                TotalTickets = basic.TotalTickets,
                OpenTickets = basic.OpenTickets,
                InProgressTickets = basic.InProgressTickets,
                ResolvedTickets = basic.ResolvedTickets,
                ClosedTickets = basic.ClosedTickets,
                TodayTickets = basic.TodayTickets,
                ThisWeekTickets = basic.ThisWeekTickets,
                ByCategory = basic.ByCategory,
                ByPriority = basic.ByPriority,
                AverageResolutionTimeHours = basic.AverageResolutionTimeHours,
                SlaAtRiskCount = slaAtRisk.Count,
                SlaBreachedCount = slaBreached.Count,
                UnassignedCount = unassigned.Count,
                EscalatedCount = basic.OpenTickets + basic.InProgressTickets
            };
        }

        public async Task<SupportTicketDto> EscalateTicketAsync(Guid ticketId, Guid userId, EscalateTicketRequest request)
        {
            var ticket = await _ticketRepository.GetByIdWithDetailsAsync(ticketId);
            if (ticket == null)
            {
                throw new Exception("Không tìm thấy ticket.");
            }

            ticket.EscalationLevel = request.EscalationLevel;
            ticket.LastEscalatedAt = DateTime.UtcNow;
            ticket.EscalatedByUserId = userId;
            ticket.EscalationReason = request.Reason;
            ticket.UpdatedAt = DateTime.UtcNow;

            // Also update SLA status to AtRisk when escalated
            if (ticket.SlaStatus == "OnTrack")
            {
                ticket.SlaStatus = "AtRisk";
            }

            var updated = await _ticketRepository.UpdateAsync(ticket);

            // Add system reply for escalation
            var escalationReply = new SupportTicketReply
            {
                TicketId = ticketId,
                SenderId = userId,
                SenderRole = "System",
                Content = $"[ESCALATION - Level {request.EscalationLevel}] {request.Reason}",
                IsInternal = true,
                CreatedAt = DateTime.UtcNow
            };
            await _replyRepository.CreateAsync(escalationReply);

            var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
            return MapToDto(updated, customer ?? ticket.Customer);
        }

        public async Task<List<SupportTicketDto>> GetSlaAtRiskAsync()
        {
            var tickets = await _ticketRepository.GetSlaAtRiskAsync();
            var result = new List<SupportTicketDto>();
            foreach (var ticket in tickets)
            {
                var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
                result.Add(MapToDto(ticket, customer));
            }
            return result;
        }

        public async Task<List<SupportTicketDto>> GetSlaBreachedAsync()
        {
            var tickets = await _ticketRepository.GetSlaBreachedAsync();
            var result = new List<SupportTicketDto>();
            foreach (var ticket in tickets)
            {
                var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
                result.Add(MapToDto(ticket, customer));
            }
            return result;
        }

        public async Task<List<SupportTicketDto>> GetUnassignedAsync()
        {
            var tickets = await _ticketRepository.GetUnassignedAsync();
            var result = new List<SupportTicketDto>();
            foreach (var ticket in tickets)
            {
                var customer = await _userRepository.GetByIdAsync(ticket.CustomerId);
                result.Add(MapToDto(ticket, customer));
            }
            return result;
        }

        public async Task CheckAndUpdateSlaStatusAsync()
        {
            var now = DateTime.UtcNow;

            // Get all active tickets
            var allTickets = await _ticketRepository.GetAllAsync(null, null, null);
            var activeTickets = allTickets.Where(t =>
                t.Status != "Closed" && t.Status != "Resolved").ToList();

            foreach (var ticket in activeTickets)
            {
                var shouldUpdate = false;

                // Check if first response deadline is breached
                if (!ticket.FirstResponseMet &&
                    ticket.FirstResponseDeadline.HasValue &&
                    ticket.FirstResponseDeadline.Value < now)
                {
                    ticket.SlaStatus = "Breached";
                    ticket.FirstResponseMet = false;
                    shouldUpdate = true;
                }
                // Check if at risk (within 30 minutes of deadline)
                else if (!ticket.FirstResponseMet &&
                         ticket.SlaStatus == "OnTrack" &&
                         ticket.FirstResponseDeadline.HasValue)
                {
                    var timeRemaining = ticket.FirstResponseDeadline.Value - now;
                    if (timeRemaining < TimeSpan.FromMinutes(30) && timeRemaining > TimeSpan.Zero)
                    {
                        ticket.SlaStatus = "AtRisk";
                        shouldUpdate = true;
                    }
                }

                if (shouldUpdate)
                {
                    ticket.UpdatedAt = now;
                    await _ticketRepository.UpdateAsync(ticket);
                }
            }
        }

        // Canned Responses Methods
        public async Task<List<CannedResponseDto>> GetCannedResponsesAsync(string? category = null)
        {
            var responses = await _cannedResponseRepository.GetAllAsync(category);
            return responses.Select(r => new CannedResponseDto
            {
                Id = r.Id,
                Title = r.Title,
                Content = r.Content,
                Category = r.Category,
                Priority = r.Priority,
                IsActive = r.IsActive,
                SortOrder = r.SortOrder,
                CreatedAt = r.CreatedAt,
                CreatedById = r.CreatedById,
                CreatedByName = r.CreatedBy?.Name
            }).ToList();
        }

        public async Task<CannedResponseDto> CreateCannedResponseAsync(Guid createdById, CreateCannedResponseRequest request)
        {
            var response = new CannedResponse
            {
                Title = request.Title,
                Content = request.Content,
                Category = request.Category,
                Priority = request.Priority,
                SortOrder = request.SortOrder,
                CreatedById = createdById,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _cannedResponseRepository.CreateAsync(response);
            var creator = await _userRepository.GetByIdAsync(createdById);

            return new CannedResponseDto
            {
                Id = created.Id,
                Title = created.Title,
                Content = created.Content,
                Category = created.Category,
                Priority = created.Priority,
                IsActive = created.IsActive,
                SortOrder = created.SortOrder,
                CreatedAt = created.CreatedAt,
                CreatedById = created.CreatedById,
                CreatedByName = creator?.Name
            };
        }

        public async Task<CannedResponseDto> UpdateCannedResponseAsync(Guid userId, UpdateCannedResponseRequest request)
        {
            var existing = await _cannedResponseRepository.GetByIdAsync(request.Id);
            if (existing == null)
            {
                throw new Exception("Không tìm thấy template.");
            }

            existing.Title = request.Title;
            existing.Content = request.Content;
            existing.Category = request.Category;
            existing.Priority = request.Priority;
            existing.IsActive = request.IsActive;
            existing.SortOrder = request.SortOrder;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _cannedResponseRepository.UpdateAsync(existing);
            var updater = await _userRepository.GetByIdAsync(userId);

            return new CannedResponseDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Content = updated.Content,
                Category = updated.Category,
                Priority = updated.Priority,
                IsActive = updated.IsActive,
                SortOrder = updated.SortOrder,
                CreatedAt = updated.CreatedAt,
                CreatedById = updated.CreatedById,
                CreatedByName = updater?.Name
            };
        }

        public async Task<bool> DeleteCannedResponseAsync(Guid id)
        {
            return await _cannedResponseRepository.DeleteAsync(id);
        }

        // Assignment Rules Methods
        public async Task<List<TicketAssignmentRuleDto>> GetAssignmentRulesAsync()
        {
            var rules = await _assignmentRuleRepository.GetAllActiveAsync();
            return rules.Select(r => new TicketAssignmentRuleDto
            {
                Id = r.Id,
                RuleName = r.RuleName,
                Category = r.Category,
                Priority = r.Priority,
                TicketStatus = r.TicketStatus,
                IsActive = r.IsActive,
                PriorityOrder = r.Priority_Order,
                AssignedToId = r.AssignedToId,
                AssignedToName = r.AssignedTo?.Name,
                AssignedToRoleId = r.AssignedToRoleId,
                AssignedToRoleName = r.AssignedToRole?.Name,
                CreatedAt = r.CreatedAt
            }).ToList();
        }

        public async Task<TicketAssignmentRuleDto> CreateAssignmentRuleAsync(Guid createdById, CreateTicketAssignmentRuleRequest request)
        {
            var rule = new TicketAssignmentRule
            {
                RuleName = request.RuleName,
                Category = request.Category,
                Priority = request.Priority,
                TicketStatus = request.TicketStatus,
                Priority_Order = request.PriorityOrder,
                AssignedToId = request.AssignedToId,
                AssignedToRoleId = request.AssignedToRoleId,
                CreatedById = createdById,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _assignmentRuleRepository.CreateAsync(rule);
            var creator = await _userRepository.GetByIdAsync(createdById);

            return new TicketAssignmentRuleDto
            {
                Id = created.Id,
                RuleName = created.RuleName,
                Category = created.Category,
                Priority = created.Priority,
                TicketStatus = created.TicketStatus,
                IsActive = created.IsActive,
                PriorityOrder = created.Priority_Order,
                AssignedToId = created.AssignedToId,
                AssignedToName = created.AssignedTo?.Name,
                AssignedToRoleId = created.AssignedToRoleId,
                AssignedToRoleName = created.AssignedToRole?.Name,
                CreatedAt = created.CreatedAt
            };
        }

        public async Task<TicketAssignmentRuleDto> UpdateAssignmentRuleAsync(Guid ruleId, CreateTicketAssignmentRuleRequest request)
        {
            var existing = await _assignmentRuleRepository.GetByIdAsync(ruleId);
            if (existing == null)
            {
                throw new Exception("Không tìm thấy rule.");
            }

            existing.RuleName = request.RuleName;
            existing.Category = request.Category;
            existing.Priority = request.Priority;
            existing.TicketStatus = request.TicketStatus;
            existing.Priority_Order = request.PriorityOrder;
            existing.AssignedToId = request.AssignedToId;
            existing.AssignedToRoleId = request.AssignedToRoleId;
            existing.UpdatedAt = DateTime.UtcNow;

            var updated = await _assignmentRuleRepository.UpdateAsync(existing);

            return new TicketAssignmentRuleDto
            {
                Id = updated.Id,
                RuleName = updated.RuleName,
                Category = updated.Category,
                Priority = updated.Priority,
                TicketStatus = updated.TicketStatus,
                IsActive = updated.IsActive,
                PriorityOrder = updated.Priority_Order,
                AssignedToId = updated.AssignedToId,
                AssignedToName = updated.AssignedTo?.Name,
                AssignedToRoleId = updated.AssignedToRoleId,
                AssignedToRoleName = updated.AssignedToRole?.Name,
                CreatedAt = updated.CreatedAt
            };
        }

        public async Task<bool> DeleteAssignmentRuleAsync(Guid id)
        {
            return await _assignmentRuleRepository.DeleteAsync(id);
        }

        private SupportTicketDto MapToDto(SupportTicket ticket, User customer)
        {
            return new SupportTicketDto
            {
                Id = ticket.Id,
                TicketCode = ticket.TicketCode,
                Subject = ticket.Subject,
                Description = ticket.Description,
                Category = ticket.Category,
                Priority = ticket.Priority,
                Status = ticket.Status,
                CustomerId = ticket.CustomerId,
                CustomerName = customer?.Name ?? "Unknown",
                CustomerEmail = customer?.Email ?? "",
                AssignedToId = ticket.AssignedToId,
                AssignedToName = ticket.AssignedTo?.Name,
                RelatedShopId = ticket.RelatedShopId,
                RelatedShopName = ticket.RelatedShop?.ShopName,
                RelatedOrderId = ticket.RelatedOrderId,
                ResponseCount = ticket.ResponseCount,
                CreatedAt = ticket.CreatedAt,
                UpdatedAt = ticket.UpdatedAt,
                ResolvedAt = ticket.ResolvedAt,
                ClosedAt = ticket.ClosedAt,
                FirstResponseDeadline = ticket.FirstResponseDeadline,
                ResolutionDeadline = ticket.ResolutionDeadline,
                SlaStatus = ticket.SlaStatus,
                FirstResponseMet = ticket.FirstResponseMet,
                ResolutionMet = ticket.ResolutionMet,
                EscalationLevel = ticket.EscalationLevel,
                EscalationReason = ticket.EscalationReason
            };
        }
    }
}
