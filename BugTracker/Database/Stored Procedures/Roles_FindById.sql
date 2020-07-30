USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_FindById]    Script Date: 30/07/2020 09:59:58 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return the Role specified by its ID.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_FindById]
	@RoleId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Roles WHERE RoleId = @RoleId
END
GO

