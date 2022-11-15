using DigitalRaid.Models;

namespace DigitalRaid.Services.Interfaces;

public interface IDRCompanyInfoService
{
    public Task<Company> GetCompanyInfoByIdAsync(int? companyId);
    public Task<IEnumerable<DRUser>> GetAllMembersAsync(int companyId);
    public Task<IEnumerable<Project>> GetAllProjectsAsync(int companyId);
    public Task<IEnumerable<Ticket>> GetAllTicketsAsync(int companyId);
}
