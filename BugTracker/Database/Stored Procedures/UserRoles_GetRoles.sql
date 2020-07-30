USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserRoles_GetRoles]    Script Date: 30/07/2020 10:01:28 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return a collection of Roles held by the specified
--		User ID.
-- =============================================
CREATE PROCEDURE [dbo].[UserRoles_GetRoles]
	@UserId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT Roles.Name
	FROM Roles
	INNER JOIN UserRoles ON UserRoles.RoleId = Roles.RoleId
	WHERE UserRoles.UserId = @UserId
END
GO

