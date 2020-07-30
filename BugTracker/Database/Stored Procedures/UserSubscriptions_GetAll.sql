USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserSubscriptions_GetAll]    Script Date: 30/07/2020 10:03:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Get all BugReport subscriptions for the specific
--		User ID.
-- =============================================
CREATE PROCEDURE [dbo].[UserSubscriptions_GetAll]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--SELECT Roles.Name
	--FROM Roles
	--INNER JOIN UserRoles ON UserRoles.RoleId = Roles.RoleId
	--WHERE UserRoles.UserId = @UserId

	SELECT BugReport.*
	FROM BugReport
	INNER JOIN UserSubscriptions ON UserSubscriptions.BugReportId = BugReport.BugReportId
	WHERE UserSubscriptions.UserId = @UserId
END
GO

