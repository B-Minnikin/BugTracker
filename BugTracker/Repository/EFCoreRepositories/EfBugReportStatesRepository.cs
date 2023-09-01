﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfBugReportStatesRepository : EFCoreBaseRepository, IBugReportStatesRepository
{
    private readonly BugReportContext context;

    public EfBugReportStatesRepository(BugReportContext context)
    {
        this.context = context;
    }
    
    public async Task<BugState> Add(BugState bugState)
    {
        if (bugState == null) throw new ArgumentNullException(nameof(bugState));

        context.BugStates.Add(bugState);
        await context.SaveChangesAsync();

        return bugState;
    }

    public async Task<BugState> GetById(int id)
    {
        var bugState = await context.BugStates.FindAsync(id);

        return bugState;
    }

    public Task<IEnumerable<BugState>> GetAllById(int id)
    {
        var bugStates = context.BugStates.Where(bs => bs.BugReportId == id);
        return Task.FromResult(bugStates.AsEnumerable());
    }

    public async Task<BugState> GetLatestState(int bugReportId)
    {
        var bugState = await context.BugStates
            .Where(bs => bs.BugReportId == bugReportId)
            .OrderByDescending(bs => bs.Time)
            .FirstOrDefaultAsync();
        return bugState;
    }
}
