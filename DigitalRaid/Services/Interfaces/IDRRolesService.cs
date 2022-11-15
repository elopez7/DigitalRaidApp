using DigitalRaid.Models;

namespace DigitalRaid.Services.Interfaces;

public interface IDRRolesService
{
    public Task<bool> IsUserInRoleAsync(DRUser user, string roleName);

    public Task<IEnumerable<string>> GetUserRolesAsync(DRUser user);

    public Task<bool> AddUserToRoleAsync(DRUser user, string roleName);

    public Task<bool> RemoveUserFromRoleAsync(DRUser user, string roleName);

    public Task<bool> RemoveUserFromRolesAsync(DRUser user, IEnumerable<string> roles);

    public Task<IEnumerable<DRUser>> GetUsersInRoleAsync(string roleName, int companyId);

    public Task<IEnumerable<DRUser>> GetUsersNotInRoleAsync(string roleName, int companyId);

    public Task<string> GetRoleNameByIdAsync(string roleId);
}
