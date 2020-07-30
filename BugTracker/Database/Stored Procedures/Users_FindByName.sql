USE [BugTrackerDB]
GO

/****** Object:  StoredProcedure [dbo].[Users_FindByName]    Script Date: 30/07/2020 10:02:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description:	Find a User by the specified User name.
-- =============================================
CREATE PROCEDURE [dbo].[Users_FindByName]
	@NormalizedUserName nvarchar(MAX)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT * FROM dbo.Users WHERE NormalizedUserName = @NormalizedUserName
END
GO

