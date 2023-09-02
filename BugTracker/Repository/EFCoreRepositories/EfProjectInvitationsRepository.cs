using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models.ProjectInvitation;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfProjectInvitationsRepository : IProjectInvitationsRepository
{
    private readonly ProjectContext context;

    public EfProjectInvitationsRepository(ProjectContext context)
    {
        this.context = context;
    }
    
    public async Task AddPendingProjectInvitation(string emailAddress, int projectId)
    {
        var projectInvitation = new PendingProjectInvitation
        {
            EmailAddress = emailAddress,
            ProjectId = projectId
        };

        context.PendingProjectInvitations.Add(projectInvitation);
        await context.SaveChangesAsync();
    }

    public async Task DeletePendingProjectInvitation(string emailAddress, int projectId)
    {
        var projectInvitation =
            await context.PendingProjectInvitations.FirstOrDefaultAsync(pi =>
                pi.ProjectId == projectId && pi.EmailAddress == emailAddress);
        context.PendingProjectInvitations.Remove(projectInvitation);
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsEmailAddressPendingRegistration(string emailAddress, int projectId)
    {
        var pendingRegistration =
            await context.PendingProjectInvitations.AnyAsync(pi =>
                pi.ProjectId == projectId && pi.EmailAddress == emailAddress);
        return pendingRegistration;
    }

    public async Task<IEnumerable<int>> GetProjectInvitationsForEmailAddress(string emailAddress)
    {
        var projectIds = await context.PendingProjectInvitations
            .Where(pi => pi.EmailAddress == emailAddress)
            .Select(pi => pi.ProjectId)
            .ToListAsync();

        return projectIds;
    }
}
