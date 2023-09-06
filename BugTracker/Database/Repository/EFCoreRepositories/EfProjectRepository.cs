using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Database.Repository.Interfaces;
using BugTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Database.Repository.EFCoreRepositories;

public class EfProjectRepository : IProjectRepository
{
    private readonly ApplicationContext context;

    public EfProjectRepository(ApplicationContext context)
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
