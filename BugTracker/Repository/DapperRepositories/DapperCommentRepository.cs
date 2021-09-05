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
		public BugReportComment Add(BugReportComment bugReportComment)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var insertedCommentId = connection.ExecuteScalar("dbo.Comments_Insert", new
				{
					Author = bugReportComment.Author,
					Date = bugReportComment.Date,
					MainText = bugReportComment.MainText,
					BugReportId = bugReportComment.BugReportId
				},
					commandType: CommandType.StoredProcedure);
				BugReportComment insertedComment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = insertedCommentId });
				return insertedComment;
			}
		}

		public BugReportComment Update(BugReportComment bugReportCommentChanges)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				connection.Execute("dbo.Comments_Update", new
				{
					BugReportCommentId = bugReportCommentChanges.BugReportCommentId,
					Author = bugReportCommentChanges.Author,
					MainText = bugReportCommentChanges.MainText
				}, commandType: CommandType.StoredProcedure);
				BugReportComment updatedComment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = bugReportCommentChanges.BugReportCommentId });
				return updatedComment;
			}
		}

		public BugReportComment Delete(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var commentToDelete = this.GetById(bugReportCommentId);
				connection.Execute("dbo.Comments_DeleteById", new { BugReportCommentId = bugReportCommentId },
					commandType: CommandType.StoredProcedure);
				return commentToDelete;
			}
		}

		public BugReportComment GetById(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comment = connection.QueryFirst<BugReportComment>("dbo.Comments_GetById @BugReportCommentId", new { BugReportCommentId = bugReportCommentId });
				return comment;
			}
		}

		public IEnumerable<BugReportComment> GetAllById(int bugReportId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var comments = connection.Query<BugReportComment>("dbo.Comments_GetAll @BugReportId", new { BugReportId = bugReportId });
				return comments;
			}
		}

		public int GetCommentParentId(int bugReportCommentId)
		{
			using (IDbConnection connection = new System.Data.SqlClient.SqlConnection(Startup.ConnectionString))
			{
				var parentId = connection.QueryFirst<int>("dbo.Comments_GetParentId @BugReportCommentId", new { BugReportCommentId = bugReportCommentId });
				return parentId;
			}
		}
	}
}
