﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BugTracker.Database.Context;
using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.Repository.EFCoreRepositories;

public class EfCommentRepository : EFCoreBaseRepository, ICommentRepository
{
    private readonly CommentContext context;

    public EfCommentRepository(CommentContext context)
    {
        this.context = context;
    }
    
    public async Task<Comment> Add(Comment comment)
    {
        context.Comments.Add(comment);
        await context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment> Update(Comment comment)
    {
        context.Comments.Update(comment);
        await context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment> Delete(int id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) throw new ArgumentException($"Comment with ID {id} not found");
        comment.Hidden = true;

        context.Comments.Update(comment);
        await context.SaveChangesAsync();

        return comment;
    }

    public async Task<Comment> GetById(int id)
    {
        var comment = await context.Comments.FindAsync(id);
        if (comment == null) throw new ArgumentException($"Comment with ID {id} not found");

        return comment;
    }

    public Task<IEnumerable<Comment>> GetAllById(int id)
    {
        var comments = context.Comments.Where(c => c.CommentId == id);
        return Task.FromResult(comments.AsEnumerable());
    }

    public async Task<int> GetCommentParentId(int commentId)
    {
        var bugReportId = await context.Comments.Where(c => c.CommentId == commentId)
            .Select(c => c.BugReportId).FirstOrDefaultAsync();

        return bugReportId;
    }
}
