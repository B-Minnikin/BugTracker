USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserRoles_RemoveFromRole]    Script Date: 30/07/2020 10:01:59 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Remove the specified User ID from a Role ID
--		for a specific Project ID.
-- =============================================
CREATE PROCEDURE [dbo].[UserRoles_RemoveFromRole]
	@UserId int,
	@RoleId int,
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.UserRoles
	WHERE UserId = @UserId AND RoleId = @RoleId AND ProjectId = @ProjectId
END
GO

