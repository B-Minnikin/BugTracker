using BugTracker.Models;
using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperCommentRepository : DapperBaseRepository, ICommentRepository
	{
		public DapperCommentRepository(string connectionString) : base(connectionString) { }

		public async Task<Comment> Add(Comment comment)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var insertedCommentId = await connection.ExecuteScalarAsync("dbo.Comments_Insert", new
				{
					AuthorId = comment.AuthorId,
					Date = comment.Date,
					MainText = comment.MainText,
					BugReportId = comment.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				Comment insertedComment = await connection.QueryFirstAsync<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = insertedCommentId });
				return insertedComment;
			}
		}

		public async Task<Comment> Update(Comment commentChanges)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.Comments_Update", new
				{
					CommentId = commentChanges.CommentId,
					AuthorId = commentChanges.AuthorId,
					MainText = commentChanges.MainText
				}, commandType: CommandType.StoredProcedure);
				Comment updatedComment = await connection.QueryFirstAsync<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentChanges.CommentId });
				return updatedComment;
			}
		}

		public async Task<Comment> Delete(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var commentToDelete = await this.GetById(commentId);
				await connection.ExecuteAsync("dbo.Comments_DeleteById", new { CommentId = commentId },
					commandType: CommandType.StoredProcedure);
				return commentToDelete;
			}
		}

		public async Task<Comment> GetById(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var comment = await connection.QueryFirstAsync<Comment>("dbo.Comments_GetById @CommentId", new { CommentId = commentId });
				return comment;
			}
		}

		public async Task<IEnumerable<Comment>> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var comments = await connection.QueryAsync<Comment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
				return comments;
			}
		}

		public async Task<int> GetCommentParentId(int commentId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var parentId = await connection.QueryFirstAsync<int>("dbo.Comments_GetParentId @CommentId", new { CommentId = commentId });
				return parentId;
			}
		}
	}
}
