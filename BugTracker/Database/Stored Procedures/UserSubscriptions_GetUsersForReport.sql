USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserSubscriptions_GetUsersForReport]    Script Date: 30/07/2020 10:03:51 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return a collection of User IDs for every
--		subscription to the specified BugReport ID.
-- =============================================
CREATE PROCEDURE [dbo].[UserSubscriptions_GetUsersForReport]
	@BugReportId int
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

	SELECT UserSubscriptions.UserId
	FROM UserSubscriptions
	WHERE UserSubscriptions.BugReportId = @BugReportId
END
GO

