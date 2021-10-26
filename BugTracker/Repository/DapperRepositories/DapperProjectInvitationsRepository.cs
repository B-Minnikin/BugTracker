﻿using BugTracker.Repository.Interfaces;
using Dapper;
using System.Collections.Generic;
using System.Data;

namespace BugTracker.Repository.DapperRepositories
{
	public class DapperProjectInvitationsRepository : DapperBaseRepository, IProjectInvitationsRepository
	{
		public DapperProjectInvitationsRepository(string connectionString) : base(connectionString) { }

		public void AddPendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.ProjectInvitations_Insert", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public void DeletePendingProjectInvitation(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				connection.Execute("dbo.ProjectInvitations_Delete", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
			}
		}

		public bool IsEmailAddressPendingRegistration(string emailAddress, int projectId)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var pendingRegistration = connection.ExecuteScalar<bool>("dbo.ProjectInvitations_IsPendingRegistration", new { EmailAddress = emailAddress, ProjectId = projectId },
					commandType: CommandType.StoredProcedure);
				return pendingRegistration;
			}
		}

		public IEnumerable<int> GetProjectInvitationsForEmailAddress(string emailAddress)
		{
			using (IDbConnection connection = GetConnectionString())
			{
				var projectIds = connection.Query<int>("dbo.ProjectInvitations_GetInvitationsForEmailAddress", new { EmailAddress = emailAddress },
					commandType: CommandType.StoredProcedure);
				return projectIds;
			}
		}
	}
}