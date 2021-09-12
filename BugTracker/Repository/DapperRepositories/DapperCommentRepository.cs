using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperCommentRepository : DapperBaseRepository, ICommentRepository
	{
		public DapperCommentRepository(string connectionString) : base(connectionString) { }

		public Comment Add(Comment comment)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedCommentId = connection.ExecuteScalar("dbo.Comments_Insert", new
				{
					AuthorId = comment.AuthorId,
					Date = comment.Date,
					MainText = comment.MainText,
					BugReportId = comment.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				Comment insertedComment = connection.QueryFirst<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = insertedCommentId });
				return insertedComment;
			}
		}

		public Comment Update(Comment commentChanges)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.Comments_Update", new
				{
					CommentId = commentChanges.CommentId,
					AuthorId = commentChanges.AuthorId,
					MainText = commentChanges.MainText
				}, commandType: CommandType.StoredProcedure);
				Comment updatedComment = connection.QueryFirst<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentChanges.CommentId });
				return updatedComment;
			}
		}

		public Comment Delete(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var commentToDelete = this.GetById(commentId);
				connection.Execute("dbo.Comments_DeleteById", new { CommentId = commentId },
					commandType: CommandType.StoredProcedure);
				return commentToDelete;
			}
		}

		public Comment GetById(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var comment = connection.QueryFirst<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentId });
				return comment;
			}
		}

		public IEnumerable<Comment> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var comments = connection.Query<Comment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
				return comments;
			}
		}

		public int GetCommentParentId(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var parentId = connection.QueryFirst<int>("dbo.Comments_GetParentId @CommentId", new { CommentId = commentId });
				return parentId;
			}
		}
	}
}
