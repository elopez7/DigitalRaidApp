using DigitalRaid.Models;

namespace DigitalRaid.Services.Interfaces;

public interface IDRTicketHistoryService
{
    Task AddHistoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
    Task<IEnumerable<TicketHistory>> GetProjectTicketsHistoryAsync(int projectId, int companyId);
    Task<IEnumerable<TicketHistory>> GetCompanyTicketsHistoryAsync(int companyId);

}
