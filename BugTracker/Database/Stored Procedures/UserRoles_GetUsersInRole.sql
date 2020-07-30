USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserRoles_GetUsersInRole]    Script Date: 30/07/2020 10:01:40 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return a collection of Users belonging to a
--		Role name for a specified Project ID.
-- =============================================
CREATE PROCEDURE [dbo].[UserRoles_GetUsersInRole]
	@NormalizedName nvarchar(MAX),
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	FROM Users
	INNER JOIN UserRoles ON UserRoles.UserId = Users.Id
	INNER JOIN Roles ON Roles.RoleId = UserRoles.RoleId
	WHERE Roles.NormalizedName = @NormalizedName AND ProjectId = @ProjectId
END
GO

