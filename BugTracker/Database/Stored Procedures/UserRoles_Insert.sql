USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[UserRoles_Insert]    Script Date: 30/07/2020 10:01:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Create a new UserRole and insert it into the
--		UserRoles table.
-- =============================================
CREATE PROCEDURE [dbo].[UserRoles_Insert]
	@RoleId int,
	@UserId int,
	@ProjectId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	INSERT INTO dbo.UserRoles (RoleId, UserId, ProjectId)
	VALUES (@RoleId, @UserId, @ProjectId)
END
GO

