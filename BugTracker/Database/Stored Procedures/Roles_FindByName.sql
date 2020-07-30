USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Roles_FindByName]    Script Date: 30/07/2020 10:00:09 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Return the Role specified by its name.
-- =============================================
CREATE PROCEDURE [dbo].[Roles_FindByName]
	@NormalizedName nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Roles WHERE NormalizedName = @NormalizedName
END
GO

