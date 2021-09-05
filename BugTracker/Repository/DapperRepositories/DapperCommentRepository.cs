using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperCommentRepository : ICommentRepository
	{
		public Comment Add(Comment comment)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedCommentId = connection.ExecuteScalar("dbo.Comments_Insert", new
				{
					Author = comment.Author,
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
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.Comments_Update", new
				{
					CommentId = commentChanges.CommentId,
					Author = commentChanges.Author,
					MainText = commentChanges.MainText
				}, commandType: CommandType.StoredProcedure);
				Comment updatedComment = connection.QueryFirst<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentChanges.CommentId });
				return updatedComment;
			}
		}

		public Comment Delete(int commentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var commentToDelete = this.GetById(commentId);
				connection.Execute("dbo.Comments_DeleteById", new { CommentId = commentId },
					commandType: CommandType.StoredProcedure);
				return commentToDelete;
			}
		}

		public Comment GetById(int commentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comment = connection.QueryFirst<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentId });
				return comment;
			}
		}

		public IEnumerable<Comment> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comments = connection.Query<Comment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
				return comments;
			}
		}

		public int GetCommentParentId(int commentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var parentId = connection.QueryFirst<int>("dbo.Comments_GetParentId @CommentId", new { CommentId = commentId });
				return parentId;
			}
		}
	}
}
