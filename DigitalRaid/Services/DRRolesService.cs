using DigitalRaid.Data;
using DigitalRaid.Models;
using DigitalRaid.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DigitalRaid.Services;

public class DRRolesService : IDRRolesService
{
    private readonly ApplicationDbContext _context;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<DRUser> _userManager;

    public DRRolesService(
        ApplicationDbContext dbContext, 
        RoleManager<IdentityRole> roleManager, 
        UserManager<DRUser> userManager
        )
    {
        _context = dbContext;
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<bool> AddUserToRoleAsync(DRUser user, string roleName)
    {
        bool result = (await _userManager.AddToRoleAsync(user, roleName)).Succeeded;
        return result;
    }

    public async Task<string> GetRoleNameByIdAsync(string roleId)
    {
        IdentityRole role = _context.Roles.Find(roleId);
        string roleName = await _roleManager.GetRoleNameAsync(role);
        return roleName;
    }

    public async Task<IEnumerable<string>> GetUserRolesAsync(DRUser user)
    {
        IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);
        return userRoles;
    }

    public async Task<IEnumerable<DRUser>> GetUsersInRoleAsync(string roleName, int companyId)
    {
        IEnumerable<DRUser> dRUsers = await _userManager.GetUsersInRoleAsync(roleName);
        IEnumerable<DRUser> usersInRole = dRUsers.Where(u => u.CompanyId == companyId);
        return usersInRole;
    }

    public async Task<IEnumerable<DRUser>> GetUsersNotInRoleAsync(string roleName, int companyId)
    {
        IEnumerable<string> userIds = (await _userManager.GetUsersInRoleAsync(roleName)).Select(u => u.Id);
        IEnumerable<DRUser> roleUsers = _context.Users.Where(u => !userIds.Contains(u.Id));
        IEnumerable<DRUser> result = roleUsers.Where(u => u.CompanyId == companyId);
        return result;
    }

    public async Task<bool> IsUserInRoleAsync(DRUser user, string roleName)
    {
        bool result = await _userManager.IsInRoleAsync(user, roleName);
        return result;
    }

    public async Task<bool> RemoveUserFromRoleAsync(DRUser user, string roleName)
    {
        bool result = (await _userManager.RemoveFromRoleAsync(user, roleName)).Succeeded;
        return result;
    }

    public async Task<bool> RemoveUserFromRolesAsync(DRUser user, IEnumerable<string> roles)
    {
        bool result = (await _userManager.RemoveFromRolesAsync(user, roles)).Succeeded;
        return result;
    }
}
