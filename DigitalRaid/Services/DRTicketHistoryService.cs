using DigitalRaid.Data;
using DigitalRaid.Models;
using DigitalRaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalRaid.Services;

public class DRTicketHistoryService : IDRTicketHistoryService
{
    private readonly ApplicationDbContext _dbContext;

    public DRTicketHistoryService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId)
    {
        if(oldTicket == null && newTicket != null)
        {
            TicketHistory history = new()
            {
                TicketId = newTicket.Id,
                Property = "",
                OldValue = "",
                NewValue = "",
                Created = DateTimeOffset.Now,
                UserId = userId,
                Description = "New ticker created"
            };

            try
            {
                await _dbContext.TicketHistories.AddAsync(history);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
        else
        {
            if(oldTicket.Title != newTicket.Title)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property= "Title",
                    OldValue= oldTicket.Title,
                    NewValue = newTicket.Title,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description= $"New ticket title: {newTicket.Title}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            if (oldTicket.Description != newTicket.Description)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property= "Description",
                    OldValue= oldTicket.Description,
                    NewValue = newTicket.Description,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description= $"New ticket title: {newTicket.Description}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketPriorityId != newTicket.TicketPriorityId)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property= "TicketPriority",
                    OldValue= oldTicket.TicketPriority.Name,
                    NewValue = newTicket.TicketPriority.Name,
                    Created = DateTimeOffset.Now,
                    UserId = userId,
                    Description= $"New ticket title: {newTicket.TicketPriority.Name}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            if(oldTicket.TicketStatusId != newTicket.TicketStatusId)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "TicketStatus",
                    OldValue = oldTicket.TicketStatus.Name,
                    NewValue = newTicket.TicketStatus.Name,
                    Created = DateTimeOffset.Now,
                    UserId= userId,
                    Description = $"New ticket status: {newTicket.TicketStatus.Name}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            if (oldTicket.TicketTypeId != newTicket.TicketTypeId)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "TicketTypeId",
                    OldValue = oldTicket.TicketType.Name,
                    NewValue = newTicket.TicketType.Name,
                    Created = DateTimeOffset.Now,
                    UserId= userId,
                    Description = $"New ticket status: {newTicket.TicketType.Name}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            if (oldTicket.DeveloperUserId != newTicket.DeveloperUserId)
            {
                TicketHistory history = new()
                {
                    TicketId = newTicket.Id,
                    Property = "TicketStatus",
                    OldValue = oldTicket.DeveloperUser?.FullName ?? "Not assigned",
                    NewValue = newTicket.DeveloperUser?.FullName,
                    Created = DateTimeOffset.Now,
                    UserId= userId,
                    Description = $"New ticket status: {newTicket.DeveloperUser.FullName}"
                };

                await _dbContext.TicketHistories.AddAsync(history);
            }

            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception) 
            {
                throw;
            }
        }
    }

    public async Task<IEnumerable<TicketHistory>> GetCompanyTicketsHistoryAsync(int companyId)
    {
        try
        {
            IEnumerable<Project> projects = (await _dbContext.Companies
                                                            .Include(c => c.Projects)
                                                                .ThenInclude(p => p.Tickets)
                                                                    .ThenInclude(t => t.History)
                                                                        .ThenInclude(h => h.User)
                                                            .FirstOrDefaultAsync(c => c.Id == companyId)).Projects;

            IEnumerable<Ticket> tickets = projects.SelectMany(p => p.Tickets);

            IEnumerable<TicketHistory> ticketHistories = tickets.SelectMany(t => t.History);

            return ticketHistories;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<TicketHistory>> GetProjectTicketsHistoryAsync(int projectId, int companyId)
    {
        try
        {
            Project project = (await _dbContext.Projects.Where(p => p.CompanyId == companyId)
                                                       .Include(p => p.Tickets)
                                                            .ThenInclude(t =>t.History)
                                                                .ThenInclude(h =>h.User)
                                                       .FirstOrDefaultAsync(p => p.Id == projectId));

            IEnumerable<TicketHistory> ticketHistories = project.Tickets.SelectMany(t => t.History);

            return ticketHistories;
        }
        catch (Exception)
        {

            throw;
        }
    }
}
