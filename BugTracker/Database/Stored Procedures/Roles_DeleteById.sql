USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_DeleteById]    Script Date: 30/07/2020 09:59:49 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Delete the role specified by ID from the Roles
--		table.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_DeleteById]
	@RoleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	DELETE FROM dbo.Roles
	WHERE RoleId = @RoleId
END
GO

