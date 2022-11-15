using DigitalRaid.Data;
using DigitalRaid.Models;
using DigitalRaid.Models.Enums;
using DigitalRaid.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DigitalRaid.Services
{
    public class DRProjectService : IDRProjectService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDRRolesService _rolesService;

        public DRProjectService(ApplicationDbContext dbContext, IDRRolesService rolesService)
        {
            _dbContext = dbContext;
            _rolesService = rolesService;
        }

        public async Task AddNewProjectAsync(Project project)
        {
            _dbContext.Add(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> AddProjectManagerAsync(string userId, int projectId)
        {
            DRUser currentProjectManager = await GetProjectManagerAsync(projectId);

            if(currentProjectManager != null)
            {
                try
                {
                    await RemoveProjectManagerAsync(projectId);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"*** ERROR *** - Error removing current PM. --> {ex.Message} <--");
                    return false;
                }
            }

            try
            {
                await AddUserToProjectAsync(userId, projectId);
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error adding new PM. --> {ex.Message} <--");
                return false;
            }

        }

        public async Task<bool> AddUserToProjectAsync(string userId, int projectId)
        {
            DRUser user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);

            if(user != null)
            {
                Project project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);
                if(!await IsUserOnProjectAsync(userId, projectId))
                {
                    try
                    {
                        project.Members.Add(user);
                        await _dbContext.SaveChangesAsync();
                        return true;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                return false;
            }

            return false;

        }

        public async Task ArchiveProjectAsync(Project project)
        {
            project.Archived = true;
            _dbContext.Update(project);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<DRUser>> GetAllProjectMembersExceptPMAsync(int projectId)
        {
            IEnumerable<DRUser> developers = await GetProjectMembersByRoleAsync(projectId, Roles.Developer.ToString());
            IEnumerable<DRUser> submitters = await GetProjectMembersByRoleAsync(projectId, Roles.Submitter.ToString());
            IEnumerable<DRUser> admins = await GetProjectMembersByRoleAsync(projectId, Roles.Admin.ToString());

            return developers.Concat(submitters.Concat(admins));
        }

        public async Task<IEnumerable<Project>> GetAllProjectsByCompany(int companyId)
        {
            IEnumerable<Project> projects = await _dbContext.Projects.Where(p => p.CompanyId == companyId && p.Archived == false)
                                                              .Include(p => p.Members)
                                                              .Include(p => p.Tickets)             
                                                                   .ThenInclude(t => t.Comments)   
                                                              .Include(p => p.Tickets)
                                                                   .ThenInclude(t => t.Attachments)
                                                              .Include(p => p.Tickets)
                                                                   .ThenInclude(t => t.History)
                                                              .Include(p => p.Tickets)
                                                                   .ThenInclude(t => t.Notifications)
                                                              .Include(p => p.Tickets)
                                                                   .ThenInclude(t => t.DeveloperUser)
                                                              .Include(p => p.Tickets)
                                                                   .ThenInclude(t => t.OwnerUser)
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

        public async Task<IEnumerable<Project>> GetAllProjectsByPriority(int companyId, string priorityName)
        {
            IEnumerable<Project> projects = await GetAllProjectsByCompany(companyId);
            int priorityId = await LookupProjectPriorityIdAsync(priorityName);
            return projects.Where(p => p.ProjectPriorityId == priorityId);
        }

        public async Task<IEnumerable<Project>> GetArchivedProjectsByCompany(int companyId)
        {
            IEnumerable<Project> projects = await GetAllProjectsByCompany(companyId);

            return projects.Where(p => p.Archived == true);
        }

        public Task<IEnumerable<DRUser>> GetDevelopersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Project> GetProjectByIdAsync(int projectId, int companyId)
        {
            Project project = await _dbContext.Projects
                                              .Include(p=>p.Tickets)
                                              .Include(p=>p.Members)
                                              .Include(p=>p.ProjectPriority)
                                              .FirstOrDefaultAsync(p => p.Id == projectId && p.CompanyId == companyId);

            return project;
        }

        public async Task<DRUser> GetProjectManagerAsync(int projectId)
        {
            Project project = await _dbContext.Projects
                                              .Include(p => p.Members)
                                              .FirstOrDefaultAsync(p => p.Id == projectId);

            foreach(DRUser member in project?.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                {
                    return member;
                }
            }
            return null;
        }

        public async Task<IEnumerable<DRUser>> GetProjectMembersByRoleAsync(int projectId, string role)
        {
            Project project = await _dbContext.Projects
                                              .Include(p => p.Members)
                                              .FirstOrDefaultAsync(p => p.Id == projectId);

            List<DRUser> members = new();

            foreach(var user in project.Members)
            {
                if(await _rolesService.IsUserInRoleAsync(user, role))
                {
                    members.Add(user);
                }
            }

            return members;
        }

        public Task<IEnumerable<DRUser>> GetSubmittersOnProjectAsync(int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Project>> GetUserProjectsAsync(string userId)
        {
            try
            {
                IEnumerable<Project> userProjects = (await _dbContext.Users
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Company)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Members)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                                   .ThenInclude(t => t.DeveloperUser)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                                   .ThenInclude(t => t.OwnerUser)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                                   .ThenInclude(t => t.TicketPriority)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                                   .ThenInclude(t => t.TicketStatus)
                                                           .Include(u => u.Projects)
                                                               .ThenInclude(p => p.Tickets)
                                                                   .ThenInclude(t => t.TicketType)
                                                           .FirstOrDefaultAsync(u => u.Id == userId)).Projects;

                return userProjects;
            }
            catch(Exception ex)
            {
                Console.WriteLine($"*** ERROR *** - Error Getting user projects list. --> {ex.Message} <--");
                throw;
            }
        }

        public async Task<IEnumerable<DRUser>> GetUsersNotOnProjectAsync(int projectId, int companyId)
        {
            IEnumerable<DRUser> users = await _dbContext.Users.Where(u => u.Projects.All(p => p.Id != projectId)).ToListAsync();

            return users.Where(u => u.CompanyId == companyId);
        }

        public async Task<bool> IsUserOnProjectAsync(string userId, int projectId)
        {
            Project project = await _dbContext.Projects.Include(p => p.Members).FirstOrDefaultAsync(p => p.Id == projectId);

            bool result = false;

            if(project != null)
            {
                result = project.Members.Any(m => m.Id == userId);
            }

            return result;
        }

        public async Task<int> LookupProjectPriorityIdAsync(string priorityName)
        {
            int priorityId = (await _dbContext.ProjectPriorities.FirstOrDefaultAsync(p => p.Name == priorityName)).Id;
            return priorityId;
        }

        public async Task RemoveProjectManagerAsync(int projectId)
        {
            Project project = await _dbContext.Projects
                                               .Include(p => p.Members)
                                               .FirstOrDefaultAsync(p => p.Id == projectId);

            try
            {
                foreach(DRUser member in project?.Members)
                {
                    if (await _rolesService.IsUserInRoleAsync(member, Roles.ProjectManager.ToString()))
                    {
                        await RemoveUserFromProjectAsync(member.Id, projectId);
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        public async Task RemoveUserFromProjectAsync(string userId, int projectId)
        {
            try
            {
                DRUser dRUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
                Project project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                try
                {
                    if (await IsUserOnProjectAsync(userId, projectId))
                    {
                        project.Members.Remove(dRUser);
                        await _dbContext.SaveChangesAsync();
                    }
                }
                catch(Exception)
                {
                    throw;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Removing User from project. ---> {ex.Message}. <---");
                throw;
            }
        }

        public async Task RemoveUsersFromProjectByRoleAsync(string role, int projectId)
        {
            try
            {
                IEnumerable<DRUser> members = await GetProjectMembersByRoleAsync(projectId, role);
                Project project = await _dbContext.Projects.FirstOrDefaultAsync(p => p.Id == projectId);

                foreach(DRUser drUser in members)
                {
                    try
                    {
                        project.Members.Remove(drUser);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine($"***** ERROR ***** - Error Removing User from project. ---> {ex.Message}. <---");
                throw;
            }
        }

        public async Task UpdateProjectAsync(Project project)
        {
            _dbContext.Update(project);
            await _dbContext.SaveChangesAsync();
        }
    }
}
