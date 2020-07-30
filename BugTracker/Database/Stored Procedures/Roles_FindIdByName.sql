USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_FindIdByName]    Script Date: 30/07/2020 10:00:21 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Takes a Role name and returns the ID of the
--		associated Role.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_FindIdByName]
	@NormalizedName nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT RoleId FROM dbo.Roles WHERE NormalizedName = @NormalizedName
END
GO

