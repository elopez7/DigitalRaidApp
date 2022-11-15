using DigitalRaid.Data;
using DigitalRaid.Models;
using DigitalRaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalRaid.Services;

public class DRCompanyInfoService : IDRCompanyInfoService
{
    private readonly ApplicationDbContext _dbContext;
    

    public DRCompanyInfoService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //If this does not work, consider returning List<DRUser> instead of IEnumerable<DRUser>
    public async Task<IEnumerable<DRUser>> GetAllMembersAsync(int companyId)
    {
        IEnumerable<DRUser> members = await _dbContext.Users.Where(u => u.CompanyId == companyId).ToListAsync();
        return members;
    }

    public async Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId)
    {
        //In Projects we have a relationship with tickets.
        //There are many tickets that have a project ID associated with it.
        //A ticket has to have a project ID, otherwise we can't create it.
        IEnumerable<Project> projects = await _dbContext.Projects.Where(p => p.CompanyId == companyId)
                                                               .Include(p => p.Members)
                                                               .Include(p => p.Tickets)             //Look for a particular project and include its associated tickets
                                                                    .ThenInclude(t => t.Comments)   //And for each of those tickets, then include its comments
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.Attachments)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.History)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.Notifications)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.DeveloperUser)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t =>t.OwnerUser)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketStatus)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketPriority)
                                                               .Include(p => p.Tickets)
                                                                    .ThenInclude(t => t.TicketType)
                                                               .Include(p => p.ProjectPriority)
                                                               .ToListAsync();
        return projects;
    }

    public async Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId)
    {
        IEnumerable<Project> projects = await GetAllProjectsAsync(companyId);
        
        IEnumerable<Ticket> tickets = projects.SelectMany(p => p.Tickets);
        return tickets;
    }

    public async Task<Company> GetCompanyInfoByIdAsync(int? companyId)
    {
        Company company = new();

        if(companyId != null)
        {
            company = await _dbContext.Companies
                                    .Include(c => c.Members)
                                    .Include(c => c.Projects)
                                    .Include(c => c.Invites)
                                    .FirstOrDefaultAsync(c => c.Id == companyId);
        }

        return company;
    }
}
