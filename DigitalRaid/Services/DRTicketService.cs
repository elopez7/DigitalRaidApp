using DigitalRaid.Data;
using DigitalRaid.Models;
using DigitalRaid.Models.Enums;
using DigitalRaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DigitalRaid.Services;

public class DRTicketService : IDRTicketService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly IDRRolesService _drRoleService;
    private readonly IDRProjectService _projectService;

    public DRTicketService(ApplicationDbContext dbContext, IDRRolesService drRoleService, IDRProjectService projectService)
    {
        _dbContext = dbContext;
        _drRoleService = drRoleService;
        _projectService = projectService;
    }

    public async Task AddNewTicketAsync(Ticket ticket)
    {
        try
        {
            _dbContext.Add(ticket);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task ArchiveTicketAsync(Ticket ticket)
    {
        try
        {
            ticket.Archived = true;
            await UpdateTicketAsync(ticket);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task AssignTicketAsync(int ticketId, string userId)
    {
        Ticket ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        try
        {
            if(ticket != null)
            {
                try
                {
                    ticket.DeveloperUserId = userId;
                    //Revisit this code when assigning tickets
                    ticket.TicketStatusId = (await LookupTicketStatusIdAsync("Developments")).Value;
                    await _dbContext.SaveChangesAsync();
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
    {
        try
        {
            IEnumerable<Ticket> tickets = await _dbContext.Projects
                                                          .Where(p => p.CompanyId == companyId)
                                                          .SelectMany(p => p.Tickets)
                                                            .Include(t => t.Attachments)
                                                            .Include(t => t.Comments)
                                                            .Include(t => t.DeveloperUser)
                                                            .Include(t => t.History)
                                                            .Include(t => t.OwnerUser)
                                                            .Include(t => t.TicketPriority)
                                                            .Include(t => t.TicketStatus)
                                                            .Include(t => t.TicketType)
                                                            .Include(t => t.Project)
                                                          .ToListAsync();

            return tickets;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
    {
        int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;

        try
        {
            IEnumerable<Ticket> tickets = await _dbContext.Projects
                                                          .Where(p => p.CompanyId == companyId)
                                                          .SelectMany(p => p.Tickets)
                                                            .Include(t => t.Attachments)
                                                            .Include(t => t.Comments)
                                                            .Include(t => t.DeveloperUser)
                                                            .Include(t => t.OwnerUser)
                                                            .Include(t => t.TicketPriority)
                                                            .Include(t => t.TicketStatus)
                                                            .Include(t => t.TicketType)
                                                            .Include(t => t.Project)
                                                          .Where(t => t.TicketPriorityId == priorityId)
                                                          .ToListAsync();


            return tickets;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
    {
        int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;

        try
        {
            IEnumerable<Ticket> tickets = await _dbContext.Projects
                                                          .Where(p => p.CompanyId == companyId)
                                                          .SelectMany(p => p.Tickets)
                                                          .Include(t => t.Attachments)
                                                          .Include(t => t.Comments)
                                                          .Include(t => t.DeveloperUser)
                                                          .Include(t => t.OwnerUser)
                                                          .Include(t => t.TicketPriority)
                                                          .Include(t => t.TicketStatus)
                                                          .Include(t => t.TicketType)
                                                          .Include(t => t.Project)
                                                        .Where(t => t.TicketStatusId == statusId)
                                                        .ToListAsync();

            return tickets;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
    {
        int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;

        try
        {
            IEnumerable<Ticket> tickets = await _dbContext.Projects
                                                          .Where(p => p.CompanyId == companyId)
                                                          .SelectMany(p => p.Tickets)
                                                          .Include(t => t.Attachments)
                                                          .Include(t => t.Comments)
                                                          .Include(t => t.DeveloperUser)
                                                          .Include(t => t.OwnerUser)
                                                          .Include(t => t.TicketPriority)
                                                          .Include(t => t.TicketStatus)
                                                          .Include(t => t.TicketType)
                                                          .Include(t => t.Project)
                                                        .Where(t => t.TicketTypeId == typeId)
                                                        .ToListAsync();

            return tickets;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(int companyId)
    {
        try
        {
            IEnumerable<Ticket> tickets = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.Archived == true);
            return tickets;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
    {
        IEnumerable<Ticket> tickets = Enumerable.Empty<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByPriorityAsync(companyId, priorityName)).Where(t => t.ProjectId == projectId);
            return tickets;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
    {
        IEnumerable<Ticket> tickets = Enumerable.Empty<Ticket>();

        try
        {
            tickets = (await GetTicketsByRoleAsync(role, userId, companyId)).Where(t => t.ProjectId == projectId);
            return tickets;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
    {
        IEnumerable<Ticket> tickets= Enumerable.Empty<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByStatusAsync(companyId, statusName)).Where(t => t.ProjectId == projectId  );
            return tickets;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
    {
        IEnumerable<Ticket> tickets= Enumerable.Empty<Ticket>();

        try
        {
            tickets = (await GetAllTicketsByTypeAsync(companyId, typeName)).Where(t => t.ProjectId == projectId);
            return tickets;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<Ticket> GetTicketByIdAsync(int ticketId)
    {
        try
        {
            return await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<DRUser> GetTicketDeveloperAsync(int ticketId, int companyId)
    {
        DRUser developer = new();

        try
        {
            Ticket ticket = (await GetAllTicketsByCompanyAsync(companyId)).FirstOrDefault(t => t.Id == ticketId);
            if(ticket?.DeveloperUserId != null)
            {
                developer = ticket.DeveloperUser;
            }
            return developer;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
    {
        IEnumerable<Ticket> ticketList = Enumerable.Empty<Ticket>();

        try
        {
            switch (role)
            {
                case nameof(Roles.Admin):
                    ticketList = await GetAllTicketsByCompanyAsync(companyId);
                    break;
                case nameof(Roles.Developer):
                    ticketList = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.DeveloperUserId == userId);
                    break;
                case nameof(Roles.Submitter):
                    ticketList = (await GetAllTicketsByCompanyAsync(companyId)).Where(t => t.OwnerUserId == userId);
                    break;
                case nameof(Roles.ProjectManager):
                    ticketList = await GetTicketsByUserIdAsync(userId, companyId);
                    break;
                default:
                    break;
            }

            return ticketList;

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
    {
        DRUser dRUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        IEnumerable<Ticket> tickets = Enumerable.Empty<Ticket>();

        try
        {
            if(await _drRoleService.IsUserInRoleAsync(dRUser, Roles.Admin.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets);
            }
            else if(await _drRoleService.IsUserInRoleAsync(dRUser, Roles.Developer.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets).Where(t => t.DeveloperUserId == userId);
            }
            else if(await _drRoleService.IsUserInRoleAsync(dRUser, Roles.Submitter.ToString()))
            {
                tickets = (await _projectService.GetAllProjectsByCompany(companyId)).SelectMany(p => p.Tickets).Where(t => t.OwnerUserId == userId);
            }
            else if(await _drRoleService.IsUserInRoleAsync(dRUser, Roles.ProjectManager.ToString()))
            {
                tickets = (await _projectService.GetUserProjectsAsync(userId)).SelectMany(p => p.Tickets);
            }
            
            return tickets;
        }
        catch (Exception)
        {

            throw;
        }
    }

    public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
    {
        try
        {
            TicketPriority priority = await _dbContext.TicketPriorities.FirstOrDefaultAsync(t => t.Name == priorityName);
            return priority?.Id;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<int?> LookupTicketStatusIdAsync(string statusName)
    {
        try
        {
            TicketStatus status = await _dbContext.TicketStatuses.FirstOrDefaultAsync(s => s.Name == statusName);
            return status?.Id;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<int?> LookupTicketTypeIdAsync(string typeName)
    {
        try
        {
            TicketType ticketType = await _dbContext.TicketTypes.FirstOrDefaultAsync(t => t.Name == typeName);
            return ticketType?.Id;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task UpdateTicketAsync(Ticket ticket)
    {
        try
        {
            _dbContext.Update(ticket);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception)
        {

            throw;
        }
    }
}
