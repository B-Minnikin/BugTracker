using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfProjectRepository : EFCoreBaseRepository, IProjectRepository
{
    private readonly ProjectContext context;

    public EfProjectRepository(ProjectContext context)
    {
        this.context = context;
    }
    
    public async Task<Project> Add(Project project)
    {
        context.Projects.Add(project);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<Project> Update(Project project)
    {
        if (project == null)
        {
            throw new ArgumentNullException(nameof(project));
        }

        context.Projects.Update(project);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<Project> Delete(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null) throw new ArgumentException($"Project with ID {id} was not found");
        
        project.Hidden = true;
        context.Projects.Update(project);
        await context.SaveChangesAsync();

        return project;
    }

    public async Task<Project> GetById(int id)
    {
        var project = await context.Projects.FindAsync(id);
        if (project == null) throw new ArgumentException($"Project with ID {id} not found");
        
        return project;
    }

    public async Task<IEnumerable<Project>> GetAll()
    {
        var projects = await context.Projects.ToListAsync();
        return projects;
    }
}
