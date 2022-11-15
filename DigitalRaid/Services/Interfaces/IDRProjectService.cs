using DigitalRaid.Models;

namespace DigitalRaid.Services.Interfaces
{
    public interface IDRProjectService
    {
        public Task AddNewProjectAsync(Project project);

        public Task<bool> AddProjectManagerAsync(string userId, int projectId);

        public Task<bool> AddUserToProjectAsync(string userId, int projectId);

        public Task ArchiveProjectAsync(Project project);

        public Task<IEnumerable<Project>> GetAllProjectsByCompany(int companyId);

        public Task<IEnumerable<Project>> GetAllProjectsByPriority(int companyId, string priorityName);

        public Task<IEnumerable<DRUser>> GetAllProjectMembersExceptPMAsync(int projectId);

        public Task<IEnumerable<Project>> GetArchivedProjectsByCompany(int companyId);

        public Task<IEnumerable<DRUser>> GetDevelopersOnProjectAsync(int projectId);

        public Task<DRUser> GetProjectManagerAsync(int projectId);

        public Task<IEnumerable<DRUser>> GetProjectMembersByRoleAsync(int projectId, string role);

        public Task<Project> GetProjectByIdAsync(int projectId, int companyId);

        public Task<IEnumerable<DRUser>> GetSubmittersOnProjectAsync(int projectId);
                    
        public Task<IEnumerable<DRUser>> GetUsersNotOnProjectAsync(int projectId, int companyId);
                    
        public Task<IEnumerable<Project>> GetUserProjectsAsync(string userId);

        public Task<bool> IsUserOnProjectAsync(string userId, int projectId);

        public Task<int> LookupProjectPriorityIdAsync(string priorityName);

        public Task RemoveProjectManagerAsync(int projectId);

        public Task RemoveUsersFromProjectByRoleAsync(string role, int projectId);

        public Task RemoveUserFromProjectAsync(string userId, int projectId);

        public Task UpdateProjectAsync(Project project);

    }
}
