USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UsersAssignedToBugReport_GetUsersForReport]    Script Date: 21/12/2020 08:12:42 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UsersAssignedToBugReport_GetUsersForReport]
	@BugReportId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT *
	FROM Users
	INNER JOIN UsersAssignedToBugReport ON UsersAssignedToBugReport.UserId = Users.Id
	WHERE UsersAssignedToBugReport.BugReportId = @BugReportId
END
GO
