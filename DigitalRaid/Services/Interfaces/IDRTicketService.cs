using DigitalRaid.Models;

namespace DigitalRaid.Services.Interfaces;

public interface IDRTicketService
{
    // CRUD Methods
    public Task AddNewTicketAsync(Ticket ticket);
    public Task UpdateTicketAsync(Ticket ticket);
    public Task<Ticket> GetTicketByIdAsync(int ticketId);
    public Task ArchiveTicketAsync(Ticket ticket);

    public Task AssignTicketAsync(int ticketId, string userId);
    public Task<IEnumerable<Ticket>> GetArchivedTicketsAsync(int companyId);
    public Task<IEnumerable<Ticket>> GetAllTicketsByCompanyAsync(int companyId);
    public Task<IEnumerable<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName);
    public Task<IEnumerable<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName);
    public Task<IEnumerable<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName);
    public Task<DRUser> GetTicketDeveloperAsync(int ticketId, int companyId);
    public Task<IEnumerable<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId);
    public Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId);
    public Task<IEnumerable<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId);
    public Task<IEnumerable<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId);
    public Task<IEnumerable<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId);
    public Task<IEnumerable<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId);


    public Task<int?> LookupTicketPriorityIdAsync(string priorityName);
    public Task<int?> LookupTicketStatusIdAsync(string statusName);
    public Task<int?> LookupTicketTypeIdAsync(string typeName);
}
