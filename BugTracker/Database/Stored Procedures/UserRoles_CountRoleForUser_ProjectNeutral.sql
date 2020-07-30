USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserRoles_CountRoleForUser_ProjectNeutral]    Script Date: 30/07/2020 10:01:16 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return count of instances of the specific User
--		in a Role across all Projects.
-- =============================================
CREATE PROCEDURE [dbo].[UserRoles_CountRoleForUser_ProjectNeutral]
	@UserId int,
	@RoleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT COUNT(*)
	FROM UserRoles
	WHERE UserId = @UserId AND RoleId = @RoleId
END
GO

