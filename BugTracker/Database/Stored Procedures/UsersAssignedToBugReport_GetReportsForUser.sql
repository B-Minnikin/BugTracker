USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UsersAssignedToBugReport_GetReportsForUser]    Script Date: 21/12/2020 08:09:22 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[UsersAssignedToBugReport_GetReportsForUser]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	SELECT UsersAssignedToBugReport.BugReportId
	FROM UsersAssignedToBugReport
	WHERE UsersAssignedToBugReport.UserId = @UserId
END
GO


